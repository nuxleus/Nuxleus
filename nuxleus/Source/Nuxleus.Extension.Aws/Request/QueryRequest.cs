﻿using System;
using System.Collections.Generic;
using Nuxleus.Extension.AWS.SimpleDB;
using Nuxleus.MetaData;

namespace Nuxleus.Extension.AWS.SimpleDB {

    public struct QueryRequest : IRequest {

        String m_requestBody;

        #region IRequest Members

        public KeyValuePair<string, string>[] Headers {
            get {
                return
                    new KeyValuePair<string, string>[] {
                        new KeyValuePair<string,string>("SOAPAction", LabelAttribute.FromMember(RequestType)),
                    };
            }
        }

        public RequestType RequestType {
            get {
                return RequestType.Query;
            }
        }

        public String RequestMessage {
            get {
                return m_requestBody;
            }
            set {
                m_requestBody = value;
            }
        }

        #endregion
    }
}
