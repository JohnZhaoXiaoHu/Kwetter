﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using Kwetter.Services.FollowService.Domain.AggregatesModel.FollowAggregate;

namespace Kwetter.Services.FollowService.API.Application.Commands.CreateFollowCommand
{
    /// <summary>
    /// Represents the <see cref="CreateFollowCommandValidator"/> class.
    /// </summary>
    public sealed class CreateFollowCommandValidator : AbstractValidator<CreateFollowCommand>
    {
        private readonly IFollowRepository _followRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFollowCommandValidator"/> class.
        /// </summary>
        /// <param name="followRepository">The follow repository.</param>
        public CreateFollowCommandValidator(IFollowRepository followRepository)
        {
            _followRepository = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
            
            RuleFor(createFollowCommand => createFollowCommand)
                .CustomAsync(ValidateFollowAsync);
        }

        private async Task ValidateFollowAsync(CreateFollowCommand createFollowCommand, CustomContext context, CancellationToken cancellationToken)
        {
            if (createFollowCommand.FollowerId == Guid.Empty)
            {
                context.AddFailure("The follower id can not be empty.");
                return;
            }
            if (createFollowCommand.FollowingId == Guid.Empty)
            {
                context.AddFailure("The following id can not be empty.");
                return;
            }
            FollowAggregate follow = await _followRepository.FindAsync(createFollowCommand.FollowingId, createFollowCommand.FollowerId, cancellationToken);
            if (follow != null) 
                context.AddFailure("The follow already exists.");
        }
    }
}