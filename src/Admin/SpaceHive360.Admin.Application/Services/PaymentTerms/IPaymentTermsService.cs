using SpaceHive360.Admin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.PaymentTerms
{
    public interface IPaymentTermsService
    {
        Task<List<PaymentTermsDTO>> GetAllPaymentTermsAsync(Guid userRecId);

        Task<PaymentTermsDTO?> GetByIdAsync(Guid recId, Guid userRecId);

        Task<Guid> CreateAsync(PaymentTermsCreateDTO dto, Guid userRecId);
        Task<bool> UpdateAsync(PaymentTermsUpdateDTO dto, Guid userRecId);
        Task<bool> DeleteAsync(Guid recId, Guid userRecId);
    }
}
