﻿using FluentValidation;
using MediatR;
using Offers.API.Domain.Validators;
using System;

namespace Offers.API.Application.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .SetValidator(new CategoryNameValidator());
        }
    }
}
