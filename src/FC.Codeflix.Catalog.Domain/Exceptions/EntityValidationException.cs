using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Exceptions;

public class EntityValidationException : Exception
{
    public IReadOnlyCollection<ValidationError>? Errors { get; }
    
    public EntityValidationException(
        string? message, 
        IReadOnlyCollection<ValidationError>? errors = null
    ) : base(message) 
        => Errors = errors;
}
