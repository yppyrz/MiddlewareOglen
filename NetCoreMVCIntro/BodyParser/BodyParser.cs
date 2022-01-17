using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCIntro.BodyParser
{
    public static class BodyParser
    {

            public static TObject Parse<TObject>(string data)
            {
                //
                var type = typeof(TObject);
                var @object = (TObject)Activator.CreateInstance(type);
                //username=techbos&password=Pa%24%24w0rd

                var formDatas = data.Split("&");

                foreach (var formData in formDatas)
                {
                    var propAndValue = formData.Split("=");

                    foreach (var prop in @object.GetType().GetProperties())
                    {
                        if (prop.Name == propAndValue[0])
                            prop.SetValue(@object, propAndValue[1]);

                    }
                }


                return @object;


            }
        }
    
}
