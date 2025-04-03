using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Cosacs.Web.Controllers
{
    public abstract class HttpHubSubscriberController<T> : Controller where T : class
    {
        public void Index()
        {
            var id = int.Parse(this.Request.Headers["X-Hub-Message-Id"]);

            using (var stream = this.Request.InputStream)
            {
                var message = (T)new XmlSerializer(typeof(T)).Deserialize(stream);
                this.Sink(id, message);
            }
        }

        protected abstract void Sink(int id, T message);
    }
}