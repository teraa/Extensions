using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Teraa.Extensions.AspNetCore;

[PublicAPI]
public sealed class EmptyStringMetadataProvider : IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context.Key.ModelType == typeof(string))
            context.DisplayMetadata.ConvertEmptyStringToNull = false;
    }
}
