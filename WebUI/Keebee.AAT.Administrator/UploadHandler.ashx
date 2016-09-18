<%@ WebHandler Language="C#" Class="TheUploadHandler" %>

using System;
using System.IO;
using System.Web;
using CuteWebUI;

public class TheUploadHandler : CuteWebUI.MvcHandler
{
    public override UploaderValidateOption GetValidateOption()
    {
        CuteWebUI.UploaderValidateOption option = new UploaderValidateOption
                                                  {
                                                      MaxSizeKB = 2147483647,
                                                      AllowedFileExtensions = "*.jpg,*.gif,*.png,*.bmp,*.mp3,*.mp4"
                                                  };
        return option;
    }

    public override void OnFileUploaded(MvcUploadFile file)
    {
        if (string.Equals(Path.GetExtension(file.FileName), ".bmp", StringComparison.OrdinalIgnoreCase))
        {
            file.Delete();
            throw(new Exception("My custom validation error : do not upload bmp"));
        }

        this.SetServerData("this value will pass to javascript api as item.ServerData");

        //  TODO:use methods
        //  to move the file to somewhere
        //file.MoveTo("~/newfolder/" + file.FileName);
        
        //  or move to somewhere
        //file.CopyTo("~/newfolder/" + file.FileName);
        
        //  or delete it
        //file.Delete()

        //get the file properties
        //file.FileGuid
        //file.FileSize
        //file.FileName

        //use this method to open an file stream
        //file.OpenStream
                
    }

    public override void OnUploaderInit(MvcUploader uploader)
    {

    }
}





