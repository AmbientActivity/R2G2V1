using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Administrator.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class AmbientInvitationsController : Controller
    {
        // api client
        private readonly IAmbientInvitationsClient _ambientInvitationsClient;

        public AmbientInvitationsController()
        {
            _ambientInvitationsClient = new AmbientInvitationsClient();
        }

        // GET: AmbientInvitations
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            string errMsg = null;
            var ambientInvitations = new AmbientInvitation[0];

            try
            {
                ambientInvitations = _ambientInvitationsClient.Get().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                AmbientInvitationList = ambientInvitations.Select(x => new AmbientInvitationViewModel
                {
                    Id = x.Id,
                    Message = x.Message,
                    IsExecuteRandom = x.IsExecuteRandom
                }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Validate(AmbientInvitationViewModel ambientInvitation)
        {
            IEnumerable<string> validateMsgs = null;
            string errMsg = null;

            try
            {
                validateMsgs = AmbientInvitationRules.Validate(ambientInvitation.Message);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ValidationMessages = validateMsgs,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Save(AmbientInvitationViewModel ambientInvitation)
        {
            string errMsg;
            AmbientInvitationViewModel vm = null;

            try
            {
                var ambientInvitationId = ambientInvitation.Id;

                errMsg = ambientInvitationId > 0
                    ? Update(ambientInvitation)
                    : Add(ambientInvitation, out ambientInvitationId);

                if (ambientInvitationId > 0)
                {
                    var invitation = _ambientInvitationsClient.Get(ambientInvitationId);
                    vm = GetViewModel(invitation);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                AmbientInvitationList = vm != null
                    ? new Collection<AmbientInvitationViewModel> { vm }
                    : new Collection<AmbientInvitationViewModel> { new AmbientInvitationViewModel() }

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id)
        {
            string errMsg;
            var deletedId = 0;

            try
            {
                errMsg = _ambientInvitationsClient.Delete(id);

                deletedId = id;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                ErrorMessage = errMsg,
                Success = string.IsNullOrEmpty(errMsg),
                DeletedId = deletedId,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetEditView(int id)
        {
            string errMsg;
            string html = null;

            try
            {
                var vm = LoadEditViewModel(id, out errMsg);
                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                html = this.RenderPartialViewToString("_AmbientInvitationEdit", vm);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                Html = html,
            }, JsonRequestBehavior.AllowGet);
        }

        private string Update(AmbientInvitationViewModel invitation)
        {
            string errMsg;
            try
            {
                var ai = new AmbientInvitation
                {
                    Message = invitation.Message,
                    IsExecuteRandom = invitation.IsExecuteRandom
                };

                errMsg = _ambientInvitationsClient.Patch(invitation.Id, ai);
                if (errMsg != null) return errMsg;

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        private string Add(AmbientInvitationViewModel invitation, out int newId)
        {
            newId = -1;
            string errMsg;

            try
            {
                var ai = new AmbientInvitation
                {
                    Id = invitation.Id,
                    Message = invitation.Message,
                    IsExecuteRandom = invitation.IsExecuteRandom
                };

                errMsg = _ambientInvitationsClient.Post(ai, out newId);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        private AmbientInvitationViewModel LoadEditViewModel(int id, out string errMsg)
        {
            errMsg = null;
            AmbientInvitationViewModel vm = null;

            try
            {
                var invitation = _ambientInvitationsClient.Get(id);

                vm = new AmbientInvitationViewModel
                {
                    Id = invitation?.Id ?? 0,
                    Message = (invitation != null) ? invitation.Message : string.Empty,
                    IsExecuteRandom = invitation?.IsExecuteRandom ?? false,
                };
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return vm;
        }

        private static AmbientInvitationViewModel GetViewModel(AmbientInvitation invitation)
        {
            return new AmbientInvitationViewModel
            {
                Id = invitation.Id,
                Message = invitation.Message,
                IsExecuteRandom = invitation.IsExecuteRandom
            };
        }
    }
}