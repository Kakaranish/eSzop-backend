﻿using System;
using Common.ErrorHandling;
using Common.Types;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Offers.API.Domain;

namespace Offers.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : BaseController
    {
        private readonly ExceptionHandler<OffersDomainException> _exceptionHandler;

        public ErrorController(ExceptionHandler<OffersDomainException> exceptionHandler)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        }

        [Route("error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;

            return _exceptionHandler.HandleException(exception);
        }
    }
}
