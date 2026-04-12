
using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IPaymentTermsRepository
    {
        Task<List<PaymentTerms>> GetAllPaymentTermsAsync(Guid userRecId);

        Task<PaymentTerms?> GetByIdAsync(Guid recId, Guid userRecId);

        Task<Guid> CreateAsync(PaymentTerms entity);
        Task UpdateAsync(PaymentTerms entity);
        Task DeleteAsync(PaymentTerms entity);

        Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId);

        Task SaveChangesAsync();

        //Task<Guid> CreatePaymentTermAsync(
        //    PaymentTermRequest request,
        //    Guid userRecId);
    }
}
