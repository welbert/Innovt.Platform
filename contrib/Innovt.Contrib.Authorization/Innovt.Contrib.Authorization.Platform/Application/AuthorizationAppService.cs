﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Innovt.Contrib.Authorization.Platform.Application.Commands;
using Innovt.Contrib.Authorization.Platform.Application.Dtos;
using Innovt.Contrib.Authorization.Platform.Domain.Filters;
using Innovt.Core.Exceptions;
using Innovt.Core.Utilities;
using Innovt.Core.Validation;
using Innovt.Domain.Security;

namespace Innovt.Contrib.Authorization.Platform.Application
{
    public class AuthorizationAppService : IAuthorizationAppService
    {
        private readonly IAuthorizationRepository authorizationRepository;

        public AuthorizationAppService(IAuthorizationRepository authorizationRepository)
        {
            this.authorizationRepository = authorizationRepository ?? throw new ArgumentNullException(nameof(authorizationRepository));
        }

        public async Task<Guid> AddPermission(AddPermissionCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            
            command.EnsureIsValid();

            var persistedPermission = await authorizationRepository.GetPermissionsBy(command.Domain, command.Resource, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (persistedPermission?.Count>0)
                throw new BusinessException(Messages.PermissionAlreadyExist);
            
            var permission = new Permission()
            {
                Domain = command.Domain,
                Name = command.Name,
                Resource = command.Resource
            };

            await authorizationRepository.AddPermission(permission, cancellationToken).ConfigureAwait(false);
            
            return permission.Id;
        }

        public async Task RemovePermission(RemovePermissionCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            command.EnsureIsValid();

            var permission = await authorizationRepository.GetPermissionsById(command.Id,cancellationToken).ConfigureAwait(false);

            if (permission is null)
                throw new BusinessException(Messages.PermissionNotFound);

            await authorizationRepository.RemovePermission(permission, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Guid> AddRole(AddRoleCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            
            command.EnsureIsValid();

            var persistedRole = await authorizationRepository.GetRoleByName(command.Name, cancellationToken).ConfigureAwait(false);

            if (persistedRole is not null)
                throw new BusinessException(Messages.RoleAlreadyExist);

            var role = new Role()
            {
                Name = command.Name,
                Description = command.Description
            };

            await authorizationRepository.AddRole(role, cancellationToken).ConfigureAwait(false);

            return role.Id;
        }

        public async Task RemoveRole(RemoveRoleCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            command.EnsureIsValid();

            var role = await authorizationRepository.GetRoleById(command.Id, cancellationToken).ConfigureAwait(false);

            if (role is null)
                throw new BusinessException(Messages.RoleNotFound);

            await authorizationRepository.RemoveRole(role, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Guid> AddGroup(AddGroupCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            command.EnsureIsValid();

            var groupPersisted = await authorizationRepository.GetGroupBy(command.Name,command.Domain, cancellationToken).ConfigureAwait(false);

            if (groupPersisted is not null)
                throw new BusinessException(Messages.GroupAlreadyExist);

            var group = new Group()
            {
                Name = command.Name,
                Description = command.Description,
                Domain = command.Domain
            };

            await authorizationRepository.AddGroup(group, cancellationToken).ConfigureAwait(false);

            return group.Id;
        }

        public async Task RemoveGroup(RemoveGroupCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            command.EnsureIsValid();

            var group = await authorizationRepository.GetGroupById(command.Id, cancellationToken).ConfigureAwait(false);

            if (group is null)
                throw new BusinessException(Messages.GroupNotFound);

            await authorizationRepository.RemoveGroup(group, cancellationToken).ConfigureAwait(false);
        }

        //init -  Create Database
        //Inject Controllers
        //Create a Group Admin
        //Create a Role ALL
        //Add all permissions to specific resource
        //O cara chama o init. ele pode ter um controller init 
        public async Task Init(InitCommand command, CancellationToken cancellationToken= default)
        {
            command.EnsureIsValid();

            //TODO: ServciceDomain
            //check if has user created

            var adminUser = authorizationRepository.GetAdminUser(command.Username,command.Password.Md5Hash());

            if (adminUser is null)
                throw new Exception("");


            return;

        }

        /// <summary>
        /// Initialize a module authorization. DataBase, First User Etc.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Init(string moduleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<GroupDto>> FindGroupBy(GroupFilter filter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PermissionDto>> FindPermissionBy(PermissionFilter filter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<RoleDto>> FindRoleBy(RoleFilter filter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
