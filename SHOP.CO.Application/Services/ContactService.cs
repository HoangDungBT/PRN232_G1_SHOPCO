using SHOP.CO.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IContactService
    {
        Task<bool> SubmitContactFormAsync(ContactDto dto);
    }

    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<bool> SubmitContactFormAsync(ContactDto dto)
        {
            var log = new InteractionLog
            {
                LogType = "Contact",
                Title = $"Liên hệ từ: {dto.FullName} ({dto.Email})",
                Message = dto.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _contactRepository.AddContactLog(log);
            await _contactRepository.SaveChangesAsync();
            return true;
        }
    }
}
