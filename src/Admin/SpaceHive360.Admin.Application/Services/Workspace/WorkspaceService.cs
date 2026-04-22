using System;
using System.Collections.Generic;
using System.Text;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using static SpaceHive360.Admin.Application.DTOs.Workspace;

namespace SpaceHive360.Admin.Application.Services.Workspace
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IWorkspaceRepository _repository;

        public WorkspaceService(IWorkspaceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateWorkspaceAsync(WorkspaceRequest request, Guid userRecId)
        {
            try
            {
                var company = _repository.GetCompanyDetailsByUserRecId(userRecId);
                if (company == null)
                    throw new InvalidOperationException("Company for the provided user was not found.");

                var uniqueCode = await _repository.GenerateUniqueWorkspaceCodeAsync();

                var entity = new Domain.Entities.Workspace
                {
                    RecId = Guid.NewGuid(),
                    FkCompany = company.RecId,
                    FkWorkspaceType = request.FkWorkspaceType,
                    FkLocation = request.FkLocation,
                    Name = request.Name,
                    Description = request.Description,
                    Capacity = request.Capacity,
                    IsActive = request.IsActive,
                    IsAvailable = request.IsAvailable,
                    CreatedBy = userRecId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UniqueCode = uniqueCode
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

        public async Task<Domain.Entities.Workspace?> GetWorkspaceByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Domain.Entities.Workspace>> GetAllWorkspacesAsync()
        {
            return await _repository.GetAllWorkspaceAsync();
        }

        public async Task<bool> DeleteWorkspaceAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
