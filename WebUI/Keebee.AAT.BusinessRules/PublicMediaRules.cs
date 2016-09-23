using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class PublicMediaRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        public IEnumerable<ResponseType> GetValidResponseTypes(int mediaPathTypeId)
        {
            var responseTypes = _opsClient.GetResponseTypes();

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.SlidShow);

                case MediaPathTypeId.Videos:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Television 
                        || x.Id == ResponseTypeId.Cats
                        || x.Id == ResponseTypeId.Ambient);

                case MediaPathTypeId.Music:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Radio);

                case MediaPathTypeId.Shapes:
                case MediaPathTypeId.Sounds:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.MatchingGame);

                default:
                    return new List<ResponseType>();
            }
        }
    }
}
