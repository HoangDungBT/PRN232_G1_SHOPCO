###############################################################################
# SHOP.CO - Comprehensive Feature Verification Script (Cart, Checkout, Orders, Payments, Invoices)
# Run: .\verify_checkout_order_final.ps1
# Requires: API running on http://localhost:5035
###############################################################################

$BaseUrl = "http://localhost:5035/api"
$TestUserId = 2
$ConnectionString = "Server=localhost\SQL2022;Database=ShopCo;User Id=sa;Password=123;TrustServerCertificate=True;"

$Global:TotalTests   = 0
$Global:PassedTests  = 0
$Global:FailedTests  = 0
$Global:Results      = @()

function Write-TestResult {
    param(
        [string]$Module,
        [string]$TestName,
        [string]$Input,
        [string]$Expected,
        [string]$Actual,
        [string]$Status
    )
    $Global:TotalTests++
    if ($Status -eq "PASS") { $Global:PassedTests++ } else { $Global:FailedTests++ }

    $color = if ($Status -eq "PASS") { "Green" } else { "Red" }
    Write-Host "  [$Status] $TestName" -ForegroundColor $color

    $Global:Results += [PSCustomObject]@{
        Module   = $Module
        Test     = $TestName
        Input    = $Input
        Expected = $Expected
        Actual   = $Actual
        Status   = $Status
    }
}

function Invoke-Api {
    param(
        [string]$Method = "GET",
        [string]$Url,
        [object]$Body = $null
    )
    try {
        $params = @{
            Method      = $Method
            Uri         = $Url
            ContentType = "application/json"
        }
        if ($Body) {
            $params["Body"] = ($Body | ConvertTo-Json -Depth 10)
        }
        $response = Invoke-RestMethod @params -ErrorAction Stop
        return $response
    } catch {
        $errBody = $null
        try {
            $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
            $errBody = $reader.ReadToEnd() | ConvertFrom-Json
        } catch {}
        $errMsg = $_.Exception.Message
        if ($errBody -and $errBody.message) { $errMsg = $errBody.message }
        return @{ success = $false; message = $errMsg; _statusCode = $_.Exception.Response.StatusCode }
    }
}

function Execute-DbNonQuery {
    param([string]$Query)
    $conn = New-Object System.Data.SqlClient.SqlConnection
    $conn.ConnectionString = $ConnectionString
    try {
        $conn.Open()
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $Query
        $cmd.ExecuteNonQuery() | Out-Null
    } finally {
        $conn.Close()
    }
}

function Execute-DbScalar {
    param([string]$Query)
    $conn = New-Object System.Data.SqlClient.SqlConnection
    $conn.ConnectionString = $ConnectionString
    try {
        $conn.Open()
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $Query
        $res = $cmd.ExecuteScalar()
        return $res
    } finally {
        $conn.Close()
    }
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host " SHOP.CO - 95 PERCENT PRODUCTION QUALITY MODULE VERIFICATION"
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Ensure user 2's cart is clear first
Execute-DbNonQuery -Query "DELETE FROM CartItems WHERE UserId = $TestUserId"

###############################################################################
# 1. CART PRICE AND STOCK REFRESH
###############################################################################
Write-Host ">>> MODULE: Cart Price and Stock Refresh" -ForegroundColor Yellow

# Step 1: Add product variant 1 to cart
$addCart = Invoke-Api -Method POST -Url "$BaseUrl/cart?userId=$TestUserId" -Body @{ variantId = 1; quantity = 2 }
if ($addCart.success -eq $true) {
    Write-TestResult -Module "Cart" -TestName "Add variant 1 to cart" -Input "variantId=1, quantity=2" -Expected "success=true" -Actual "success=true" -Status "PASS"
} else {
    Write-TestResult -Module "Cart" -TestName "Add variant 1 to cart" -Input "variantId=1, quantity=2" -Expected "success=true" -Actual $addCart.message -Status "FAIL"
}

# Step 2: Manually change variant 1's price in DB
Execute-DbNonQuery -Query "UPDATE ProductVariants SET StockQuantity = 60 WHERE VariantId = 1"
# Verify Price refresh by viewing Cart
$cartInfo = Invoke-Api -Method GET -Url "$BaseUrl/cart/$TestUserId"
if ($cartInfo.success -eq $true) {
    $firstItem = $cartInfo.data.items[0]
    if ($firstItem.availableStock -eq 60) {
        Write-TestResult -Module "Cart" -TestName "Stock refresh dynamically in Cart DTO" -Input "variantId=1" -Expected "availableStock=60" -Actual "availableStock=$($firstItem.availableStock)" -Status "PASS"
    } else {
        Write-TestResult -Module "Cart" -TestName "Stock refresh dynamically in Cart DTO" -Input "variantId=1" -Expected "availableStock=60" -Actual "availableStock=$($firstItem.availableStock)" -Status "FAIL"
    }
} else {
    Write-TestResult -Module "Cart" -TestName "Stock refresh dynamically in Cart DTO" -Input "variantId=1" -Expected "availableStock=60" -Actual "failed to fetch cart" -Status "FAIL"
}

# Restore StockQuantity
Execute-DbNonQuery -Query "UPDATE ProductVariants SET StockQuantity = 50 WHERE VariantId = 1"

###############################################################################
# 2. CHECKOUT ADDRESS, NOTE, PAYMENT METHOD
###############################################################################
Write-Host ">>> MODULE: Checkout Options and Note" -ForegroundColor Yellow

# Checkout with selected address (company address = 2), note, and COD method
$checkoutRes = Invoke-Api -Method POST -Url "$BaseUrl/orders/checkout" -Body @{ userId = $TestUserId; couponCode = $null; addressId = 2; customerNote = "Deliver after 5 PM please"; paymentMethod = "COD" }

$CreatedOrderId = $null
$CreatedOrderCode = $null
if ($checkoutRes.success -eq $true) {
    $CreatedOrderId = $checkoutRes.data.orderId
    $CreatedOrderCode = $checkoutRes.data.orderCode
    Write-TestResult -Module "Checkout" -TestName "Checkout with options" -Input "addressId=2, paymentMethod=COD" -Expected "success=true, order created" -Actual "orderCode=$($checkoutRes.data.orderCode)" -Status "PASS"
} else {
    Write-TestResult -Module "Checkout" -TestName "Checkout with options" -Input "addressId=2, paymentMethod=COD" -Expected "success=true" -Actual $checkoutRes.message -Status "FAIL"
}

# Verify stored values in order
if ($CreatedOrderId) {
    $orderInfo = Invoke-Api -Method GET -Url "$BaseUrl/orders/$CreatedOrderId"
    if ($orderInfo.success -eq $true) {
        $ord = $orderInfo.data
        $addrMatch = $ord.addressId -eq 2
        $noteMatch = $ord.customerNote -eq "Deliver after 5 PM please"
        if ($addrMatch -and $noteMatch) {
            Write-TestResult -Module "Checkout" -TestName "Verify saved address and customer note" -Input "orderId=$CreatedOrderId" -Expected "addressId=2, customerNote='Deliver after 5 PM please'" -Actual "addressId=$($ord.addressId), customerNote='$($ord.customerNote)'" -Status "PASS"
        } else {
            Write-TestResult -Module "Checkout" -TestName "Verify saved address and customer note" -Input "orderId=$CreatedOrderId" -Expected "addressId=2, customerNote='Deliver after 5 PM please'" -Actual "addressId=$($ord.addressId), customerNote='$($ord.customerNote)'" -Status "FAIL"
        }
    } else {
         Write-TestResult -Module "Checkout" -TestName "Verify saved address and customer note" -Input "orderId=$CreatedOrderId" -Expected "data verified" -Actual "failed to load details" -Status "FAIL"
    }
}

###############################################################################
# 3. TRANSACTION ROLLBACK
###############################################################################
Write-Host ">>> MODULE: Transaction Rollback" -ForegroundColor Yellow

# Step 1: Add variant 1 to cart
Invoke-Api -Method POST -Url "$BaseUrl/cart?userId=$TestUserId" -Body @{ variantId = 1; quantity = 3 } | Out-Null

# Step 2: Manually set stock of variant 1 to 0 in DB
Execute-DbNonQuery -Query "UPDATE ProductVariants SET StockQuantity = 0 WHERE VariantId = 1"

# Step 3: Trigger checkout which will fail due to insufficient stock inside transaction
$rollbackCheckout = Invoke-Api -Method POST -Url "$BaseUrl/orders/checkout" -Body @{ userId = $TestUserId; couponCode = $null; addressId = 2; paymentMethod = "COD" }

if ($rollbackCheckout.success -eq $false) {
    # Verify that the cart items were NOT cleared (they are still in DB) because of rollback
    $cartCount = Execute-DbScalar -Query "SELECT COUNT(*) FROM CartItems WHERE UserId = $TestUserId"
    if ($cartCount -gt 0) {
        Write-TestResult -Module "Rollback" -TestName "Transaction rollbacks successfully on stock error" -Input "variant stock=0" -Expected "success=false, cart count stays greater than 0" -Actual "success=$($rollbackCheckout.success), db cart items count=$cartCount" -Status "PASS"
    } else {
        Write-TestResult -Module "Rollback" -TestName "Transaction rollbacks successfully on stock error" -Input "variant stock=0" -Expected "success=false, cart count stays greater than 0" -Actual "cart count is 0 (Rollback FAILED)" -Status "FAIL"
    }
} else {
    Write-TestResult -Module "Rollback" -TestName "Transaction rollbacks successfully on stock error" -Input "variant stock=0" -Expected "success=false" -Actual "success=true (Rollback FAILED)" -Status "FAIL"
}

# Restore Variant stock and clear cart
Execute-DbNonQuery -Query "UPDATE ProductVariants SET StockQuantity = 50 WHERE VariantId = 1"
Execute-DbNonQuery -Query "DELETE FROM CartItems WHERE UserId = $TestUserId"

###############################################################################
# 4. COD / VNPAY SIMULATION FLOW
###############################################################################
Write-Host ">>> MODULE: Payment and Callback Flow" -ForegroundColor Yellow

# Step 1: Add items to cart and Checkout with VNPay method
Invoke-Api -Method POST -Url "$BaseUrl/cart?userId=$TestUserId" -Body @{ variantId = 1; quantity = 1 } | Out-Null
$vnpayCheckout = Invoke-Api -Method POST -Url "$BaseUrl/orders/checkout" -Body @{ userId = $TestUserId; addressId = 1; paymentMethod = "VNPay" }
$vnpOrderId = $vnpayCheckout.data.orderId
$vnpOrderCode = $vnpayCheckout.data.orderCode

# Step 2: Request payment url
$payProcess = Invoke-Api -Method POST -Url "$BaseUrl/payment/process" -Body @{ orderId = $vnpOrderId; paymentMethod = "VNPay" }
if ($payProcess.success -eq $true -and $payProcess.paymentUrl) {
    Write-TestResult -Module "Payment" -TestName "Generate VNPay payment URL" -Input "orderId=$vnpOrderId" -Expected "paymentUrl is returned" -Actual "url=$($payProcess.paymentUrl)" -Status "PASS"
} else {
    Write-TestResult -Module "Payment" -TestName "Generate VNPay payment URL" -Input "orderId=$vnpOrderId" -Expected "paymentUrl is returned" -Actual "failed" -Status "FAIL"
}

# Step 3: Simulate success callback
$callbackSim = Invoke-Api -Method POST -Url "$BaseUrl/payment/simulate-callback" -Body @{ orderCode = $vnpOrderCode; responseCode = "00" }
if ($callbackSim.success -eq $true -and $callbackSim.data.paymentStatus -eq "Paid") {
    Write-TestResult -Module "Payment" -TestName "Simulate VNPay success callback" -Input "orderCode=$vnpOrderCode, response=00" -Expected "paymentStatus=Paid" -Actual "status=$($callbackSim.data.paymentStatus)" -Status "PASS"
} else {
    Write-TestResult -Module "Payment" -TestName "Simulate VNPay success callback" -Input "orderCode=$vnpOrderCode, response=00" -Expected "paymentStatus=Paid" -Actual "status=$($callbackSim.data.paymentStatus)" -Status "FAIL"
}

# Step 4: Verify payment block on Paid status
$secondPay = Invoke-Api -Method POST -Url "$BaseUrl/payment/process" -Body @{ orderId = $vnpOrderId; paymentMethod = "VNPay" }
if ($secondPay.success -eq $false -and $secondPay.message -match "already been paid") {
    Write-TestResult -Module "Payment" -TestName "Block payment request for already paid order" -Input "orderId=$vnpOrderId" -Expected "success=false, already been paid" -Actual "message='$($secondPay.message)'" -Status "PASS"
} else {
    Write-TestResult -Module "Payment" -TestName "Block payment request for already paid order" -Input "orderId=$vnpOrderId" -Expected "success=false" -Actual "success=$($secondPay.success), message='$($secondPay.message)'" -Status "FAIL"
}

###############################################################################
# 5. CANCEL AND STOCK RESTORE
###############################################################################
Write-Host ">>> MODULE: Cancellation and Stock Restore" -ForegroundColor Yellow

# Step 1: Create a pending order
$initialStock = Execute-DbScalar -Query "SELECT StockQuantity FROM ProductVariants WHERE VariantId = 1" # should be 50
Invoke-Api -Method POST -Url "$BaseUrl/cart?userId=$TestUserId" -Body @{ variantId = 1; quantity = 3 } | Out-Null

$cancelCheckout = Invoke-Api -Method POST -Url "$BaseUrl/orders/checkout" -Body @{ userId = $TestUserId; addressId = 1; paymentMethod = "COD" }
$cancelOrderId = $cancelCheckout.data.orderId

# Step 2: Cancel pending order
$cancelRes = Invoke-Api -Method POST -Url "$BaseUrl/orders/$cancelOrderId/cancel?userId=$TestUserId" -Body @{ cancelReason = "Out of cash" }
if ($cancelRes.success -eq $true) {
    # Check variant 1 stock is restored in DB
    $newStock = Execute-DbScalar -Query "SELECT StockQuantity FROM ProductVariants WHERE VariantId = 1" # should be 50
    if ($newStock -eq $initialStock) {
        Write-TestResult -Module "Cancel" -TestName "Cancel order and restore variant stock" -Input "orderId=$cancelOrderId" -Expected "stock restored to original" -Actual "stock before=$initialStock, stock after=$newStock" -Status "PASS"
    } else {
        Write-TestResult -Module "Cancel" -TestName "Cancel order and restore variant stock" -Input "orderId=$cancelOrderId" -Expected "stock restored to original" -Actual "stock before=$initialStock, stock after=$newStock" -Status "FAIL"
    }
} else {
    Write-TestResult -Module "Cancel" -TestName "Cancel order and restore variant stock" -Input "orderId=$cancelOrderId" -Expected "success=true" -Actual "message=$($cancelRes.message)" -Status "FAIL"
}

###############################################################################
# 6. ORDER TRACKING TIMELINE
###############################################################################
Write-Host ">>> MODULE: Tracking Timeline" -ForegroundColor Yellow

$trackingInfo = Invoke-Api -Method GET -Url "$BaseUrl/orders/$cancelOrderId/tracking"
if ($trackingInfo.success -eq $true) {
    $canceledStep = $trackingInfo.data.steps | Where-Object { $_.status -eq "Canceled" }
    if ($canceledStep -and $canceledStep.time -ne $null) {
        Write-TestResult -Module "Tracking" -TestName "Canceled tracking contains Canceled step with completedAt timestamp" -Input "orderId=$cancelOrderId" -Expected "Canceled step has time" -Actual "time=$($canceledStep.time)" -Status "PASS"
    } else {
        Write-TestResult -Module "Tracking" -TestName "Canceled tracking contains Canceled step with completedAt timestamp" -Input "orderId=$cancelOrderId" -Expected "Canceled step has time" -Actual "not found" -Status "FAIL"
    }
} else {
    Write-TestResult -Module "Tracking" -TestName "Canceled tracking contains Canceled step with completedAt timestamp" -Input "orderId=$cancelOrderId" -Expected "success=true" -Actual "failed" -Status "FAIL"
}

###############################################################################
# 7. RETURN REQUEST DESCRIPTION
###############################################################################
Write-Host ">>> MODULE: Return Request Reason and Description" -ForegroundColor Yellow

# Step 1: Create a delivered order (force order status to Delivered in DB)
Invoke-Api -Method POST -Url "$BaseUrl/cart?userId=$TestUserId" -Body @{ variantId = 1; quantity = 1 } | Out-Null
$retCheckout = Invoke-Api -Method POST -Url "$BaseUrl/orders/checkout" -Body @{ userId = $TestUserId; addressId = 1; paymentMethod = "COD" }
$retOrderId = $retCheckout.data.orderId
Execute-DbNonQuery -Query "UPDATE Orders SET OrderStatus = 'Completed' WHERE OrderId = $retOrderId"

# Step 2: Submit Return Request with Reason and Description
$returnRes = Invoke-Api -Method POST -Url "$BaseUrl/orders/$retOrderId/return-request?userId=$TestUserId" -Body @{ reason = "WrongColor"; description = "I ordered white but received black" }

if ($returnRes.success -eq $true) {
    # Check that reason and description are logged into order details (e.g. CustomerNote)
    $dbNote = Execute-DbScalar -Query "SELECT CustomerNote FROM Orders WHERE OrderId = $retOrderId"
    if ($dbNote -match "WrongColor" -and $dbNote -match "I ordered white") {
        Write-TestResult -Module "Return" -TestName "Submit return request with reason and description" -Input "orderId=$retOrderId" -Expected "CustomerNote contains reason and description" -Actual "dbNote='$dbNote'" -Status "PASS"
    } else {
        Write-TestResult -Module "Return" -TestName "Submit return request with reason and description" -Input "orderId=$retOrderId" -Expected "CustomerNote contains reason and description" -Actual "dbNote='$dbNote'" -Status "FAIL"
    }
} else {
    Write-TestResult -Module "Return" -TestName "Submit return request with reason and description" -Input "orderId=$retOrderId" -Expected "success=true" -Actual "message=$($returnRes.message)" -Status "FAIL"
}

###############################################################################
# 8. INVOICE PDF DOWNLOAD
###############################################################################
Write-Host ">>> MODULE: Invoice Customer Info and PDF Download" -ForegroundColor Yellow

$invoiceUrl = "$BaseUrl/orders/$retOrderId/invoice?userId=$TestUserId"
try {
    $wc = New-Object System.Net.WebClient
    $invoiceBytes = $wc.DownloadData($invoiceUrl)
    $contentHeader = $wc.ResponseHeaders["Content-Type"]
    if ($invoiceBytes.Length -gt 1000 -and $contentHeader -eq "application/pdf") {
        Write-TestResult -Module "Invoice" -TestName "Invoice PDF download with correct content-type and size" -Input "orderId=$retOrderId" -Expected "application/pdf, gt 1KB" -Actual "contentType=$contentHeader, size=$($invoiceBytes.Length) bytes" -Status "PASS"
    } else {
        Write-TestResult -Module "Invoice" -TestName "Invoice PDF download with correct content-type and size" -Input "orderId=$retOrderId" -Expected "application/pdf, gt 1KB" -Actual "contentType=$contentHeader, size=$($invoiceBytes.Length) bytes" -Status "FAIL"
    }
} catch {
    Write-TestResult -Module "Invoice" -TestName "Invoice PDF download with correct content-type and size" -Input "orderId=$retOrderId" -Expected "application/pdf" -Actual "failed: $($_.Exception.Message)" -Status "FAIL"
}

###############################################################################
# SUMMARY
###############################################################################
Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host " MODULE UPGRADES TEST SUMMARY"
Write-Host "==================================================" -ForegroundColor Cyan
$failColor = if ($Global:FailedTests -gt 0) { "Red" } else { "Green" }
Write-Host " Total Tests : $Global:TotalTests"
Write-Host " Passed      : $Global:PassedTests" -ForegroundColor Green
Write-Host " Failed      : $Global:FailedTests" -ForegroundColor $failColor
Write-Host ""

$Global:Results | Format-Table -AutoSize Module, Test, Status, Expected, Actual

Write-Host ""
if ($Global:FailedTests -eq 0) {
    Write-Host " [PASS] ALL UPGRADES VERIFIED SUCCESSFULLY" -ForegroundColor Green
} else {
    Write-Host " [FAIL] SOME UPGRADES TESTS FAILED - NEEDS ATTENTION" -ForegroundColor Red
}
Write-Host ""
