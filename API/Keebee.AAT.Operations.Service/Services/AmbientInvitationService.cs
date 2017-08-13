using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IAmbientInvitationService
    {
        IEnumerable<AmbientInvitation> Get();
        AmbientInvitation Get(int id);
        int Post(AmbientInvitation ambientInvitation);
        void Patch(int id, AmbientInvitation ambientInvitation);
        void Delete(int id);
    }

    public class AmbientInvitationService : IAmbientInvitationService
    {
        public IEnumerable<AmbientInvitation> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var ambientInvitations = container.AmbientInvitations
                .AsEnumerable();

            return ambientInvitations;
        }

        public AmbientInvitation Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var ambientInvitation = container.AmbientInvitations.ByKey(id);

            AmbientInvitation result;
            try { result = ambientInvitation.GetValue(); }
            catch { result = null; }

            return result;
        }

        public int Post(AmbientInvitation ambientInvitation)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToAmbientInvitations(ambientInvitation);
            container.SaveChanges();

            return ambientInvitation.Id;
        }

        public void Patch(int id, AmbientInvitation ambientInvitation)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.AmbientInvitations.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (!string.IsNullOrEmpty(ambientInvitation.Message))
                el.Message = ambientInvitation.Message;

            el.IsExecuteRandom = ambientInvitation.IsExecuteRandom;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var ambientInvitation = container.AmbientInvitations.Where(e => e.Id == id).SingleOrDefault();
            if (ambientInvitation == null) return;

            container.DeleteObject(ambientInvitation);
            container.SaveChanges();
        }
    }
}
