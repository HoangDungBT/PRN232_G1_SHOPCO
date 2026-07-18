using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Repositories;


using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Application.Common;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
namespace SHOP.CO.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly SHOP.CO.Infrastructure.Repositories.IOrderRepository _orderRepository;

        static InvoiceService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public InvoiceService(SHOP.CO.Infrastructure.Repositories.IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<byte[]> GenerateInvoiceAsync(int orderId, int userId)
        {
            // 1. Get Order with Items, Variants, Product and User details loaded
            var order = await _orderRepository.GetOrderWithItemsAndVariantsByIdAsync(orderId);

            // 2. Validate
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            // User ownership
            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this invoice.");
            }

            // Validate status: Cancelled is not allowed
            if (string.Equals(order.OrderStatus, "Canceled", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Cannot generate invoice for canceled order.");
            }

            // 3. Determine Payment Method (COD / VNPay)
            var paymentRecord = order.CommerceRecords?.FirstOrDefault(cr => 
                string.Equals(cr.RecordType, "Payment", StringComparison.OrdinalIgnoreCase));
            
            string paymentMethod = "COD";
            if (paymentRecord != null)
            {
                if (string.Equals(paymentRecord.PaymentMethod, "Online", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(paymentRecord.PaymentProvider, "VNPay", StringComparison.OrdinalIgnoreCase))
                {
                    paymentMethod = "VNPay";
                }
                else if (string.Equals(paymentRecord.PaymentMethod, "COD", StringComparison.OrdinalIgnoreCase))
                {
                    paymentMethod = "COD";
                }
            }
            else if (string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                paymentMethod = "VNPay";
            }

            // 4. Map to DTO
            var customerName = order.User != null ? order.User.FullName : order.ReceiverName;
            var invoice = new InvoiceDto
            {
                InvoiceCode = $"INV-{order.OrderCode}",
                OrderCode = order.OrderCode,
                CreatedAt = order.CreatedAt,
                SubtotalAmount = order.SubtotalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                CustomerName = customerName,
                CustomerAddress = order.ShippingAddressText,
                CustomerPhone = order.ReceiverPhone,
                Items = order.OrderItems.Select(item => new InvoiceItemDto
                {
                    ProductName = item.ProductNameSnapshot,
                    Sku = item.SkuSnapshot,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.LineTotal
                }).ToList()
            };

            // 5. Generate PDF
            return GeneratePdfBytes(invoice, paymentMethod);
        }

        private byte[] GeneratePdfBytes(InvoiceDto invoice, string paymentMethod)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(11));

                    // Header: SHOP.CO
                    page.Header().Column(header =>
                    {
                        header.Item().Text("SHOP.CO").FontSize(24).Bold().FontColor(Colors.Black);
                        header.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // Body
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(8);

                        col.Item().Text("INVOICE").FontSize(20).Bold();

                        col.Item().Text(t =>
                        {
                            t.Span("Invoice Code:\n").Bold();
                            t.Span($"INV-{invoice.OrderCode}");
                        });

                        col.Item().Text(t =>
                        {
                            t.Span("Order Code:\n").Bold();
                            t.Span(invoice.OrderCode);
                        });

                        col.Item().Text(t =>
                        {
                            t.Span("Customer Details:\n").Bold();
                            t.Span($"Name: {invoice.CustomerName}\nAddress: {invoice.CustomerAddress ?? "N/A"}\nPhone: {invoice.CustomerPhone ?? "N/A"}");
                        });

                        col.Item().Text(t =>
                        {
                            t.Span("Order Date:\n").Bold();
                            t.Span(invoice.CreatedAt.ToLocalTime().ToString("f"));
                        });

                        col.Item().PaddingTop(10).Text("Items:").FontSize(14).Bold();

                        // Table of items
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Product Name
                                columns.RelativeColumn(2); // SKU
                                columns.RelativeColumn(1); // Quantity
                                columns.RelativeColumn(2); // Unit Price
                                columns.RelativeColumn(2); // Total
                            });

                            // Header row
                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Product Name").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("SKU").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantity").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Unit Price").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").Bold();
                            });

                            foreach (var item in invoice.Items)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.ProductName);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Sku);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Quantity.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{item.UnitPrice:N0} VND");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{item.Total:N0} VND");
                            }
                        });

                        col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        // Totals breakdown
                        col.Item().AlignRight().Column(totalsCol =>
                        {
                            totalsCol.Spacing(4);
                            totalsCol.Item().Text(t =>
                            {
                                t.Span("Subtotal:\n").Bold();
                                t.Span($"{invoice.SubtotalAmount:N0} VND");
                            });

                            if (invoice.DiscountAmount > 0)
                            {
                                totalsCol.Item().Text(t =>
                                {
                                    t.Span("Discount:\n").Bold();
                                    t.Span($"-{invoice.DiscountAmount:N0} VND");
                                });
                            }

                            totalsCol.Item().Text(t =>
                            {
                                t.Span("Shipping:\n").Bold();
                                t.Span($"{invoice.ShippingFee:N0} VND");
                            });

                            totalsCol.Item().Text(t =>
                            {
                                t.Span("TOTAL:\n").Bold().FontSize(13);
                                t.Span($"{invoice.TotalAmount:N0} VND").Bold().FontSize(13);
                            });
                        });

                        col.Item().Text(t =>
                        {
                            t.Span("Payment:\n").Bold();
                            t.Span(paymentMethod);
                        });

                        col.Item().Text(t =>
                        {
                            t.Span("Order Status:\n").Bold();
                            t.Span(invoice.OrderStatus);
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Column(footerCol =>
                    {
                        footerCol.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        footerCol.Item().PaddingTop(5).Text("Thank you for shopping with SHOP.CO").Italic().FontSize(10);
                    });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return stream.ToArray();
            }
        }
    }
}
