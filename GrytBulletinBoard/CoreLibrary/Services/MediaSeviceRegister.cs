using CoreLibrary.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace CoreLibrary.Composing
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class RegisterServicesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register(typeof(IMediaUploadService),
                typeof(MediaUploadService), Lifetime.Request);
            composition.Register(typeof(IContentNoteService),
                typeof(ContentNoteService), Lifetime.Request);
        }
    }
}

