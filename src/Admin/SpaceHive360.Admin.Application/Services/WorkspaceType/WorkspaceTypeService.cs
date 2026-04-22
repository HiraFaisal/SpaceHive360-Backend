using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using static SpaceHive360.Admin.Application.DTOs.Workspace;

namespace SpaceHive360.Admin.Application.Services.WorkspaceType
{
    public class WorkspaceTypeService : IWorkspaceTypeService
    {
        private readonly IWorkspaceTypeRepository _repository;

        public WorkspaceTypeService(IWorkspaceTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateWorkspaceTypeAsync(
            WorkspaceTypeRequest request,
            Guid userRecId)
        {
            try
            {
                var company = _repository.GetCompanyDetailsByUserRecId(userRecId);
                var entity = new Domain.Entities.WorkspaceType
                {
                    RecId = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    IconUrl = request.IconUrl,
                    IsActive = request.IsActive,
                    CreatedBy = userRecId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    FkCompany = company?.RecId
                };

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return entity.RecId;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Domain.Entities.WorkspaceType?> GetWorkspaceTypeByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Domain.Entities.WorkspaceType>> GetAllWorkspaceTypesAsync(Guid userRecId)
        {
            return await _repository.GetAllAsync(userRecId);
        }

        public async Task<bool> UpdateWorkspaceTypeAsync(
            Guid id,
            EditWorkspaceTypeRequest dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            entity.Name = dto.WorkspaceType.Name;
            entity.Description = dto.WorkspaceType.Description;
            entity.IconUrl = dto.WorkspaceType.IconUrl;
            entity.IsActive = dto.WorkspaceType.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            //await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWorkspaceTypeAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            // Soft delete
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;


            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
