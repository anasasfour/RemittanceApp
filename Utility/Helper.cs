﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualBasic;
using TP;

namespace RemittanceApp.Utility
{
    public class Helper
    {
        const string T_BOS_CURRENCY = "7030";
        private string GetCurrentDate()
        {
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            string sHour = DateTime.Now.Hour.ToString();
            string sMin = DateTime.Now.Minute.ToString();
            string sSec = DateTime.Now.Second.ToString();
            string sMil = DateTime.Now.Millisecond.ToString();

            return sYear + Interaction.IIf(Strings.Len(sMonth) == 1, "0" + sMonth, sMonth) + Interaction.IIf(Strings.Len(sDay) == 1, "0" + sDay, sDay) + Interaction.IIf(Strings.Len(sHour) == 1, "0" + sHour, sHour) + Interaction.IIf(Strings.Len(sMin) == 1, "0" + sMin, sMin) + Interaction.IIf(Strings.Len(sSec) == 1, "0" + sSec, sSec) + Interaction.IIf(Strings.Len(sMil) == 1, "00" + sSec, Interaction.IIf(Strings.Len(sSec) == 2, "0" + sSec, sSec));
        }

        private string PrepareXML(string strTrans_Code, string strEvent_ID, string strIP_LAddress, string strIP_HAddress, string strUser_ID, params string[] Args)
        {
            try
            {
                int intParamIndex;
                string strTime_Stamp = GetCurrentDate();


                string strXML;
                int intIndex;

                strXML = "<?xml version=\"1.0\"  encoding=\"windows-1256\"?>";
                strXML = strXML + "<ROOT> " + "<Trans ID='" + Interaction.IIf(Strings.Trim(strTrans_Code) == "", "", strTrans_Code.Replace("'", "`")) + "' " + "User_ID='" + Interaction.IIf(Strings.Trim(strUser_ID) == "", "", strUser_ID.Replace("'", "`")) + "' " + "Event_ID='" + Interaction.IIf(Strings.Trim(strEvent_ID) == "", "", strEvent_ID.Replace("'", "`")) + "' " + "IP_LAddress='" + Interaction.IIf(Strings.Trim(strIP_LAddress) == "", "", strIP_LAddress.Replace("'", "`")) + "' " + "IP_HAddress='" + Interaction.IIf(Strings.Trim(strIP_HAddress) == "", "", strIP_HAddress.Replace("'", "`")) + "' " + "Time_Stamp='" + Interaction.IIf(Strings.Trim(strTime_Stamp) == "", "", strTime_Stamp.Replace("'", "`")) + "' /> ";

                strXML = strXML + "<Params ";
                string strTemp;
                for (intIndex = 0; intIndex <= Information.UBound(Args); intIndex++)
                {
                    intParamIndex = (int)Conversion.Int((intIndex + 1) / (double)10);
                    strXML = strXML + "Param_" + intParamIndex + "_" + intIndex + 1 + "=";
                    if (Strings.Trim(Args[intIndex]) != "")
                    {
                        // -------------------------
                        // Remove ' and & & %
                        strTemp = Args[intIndex].Replace("'", "`");
                        strTemp = strTemp.Replace("&", "`");
                        strTemp = strTemp.Replace("%", "`");
                        // --------------------------
                        strXML = strXML + "'" + strTemp + "' ";
                    }
                    else
                        strXML = strXML + "'' ";
                }
                strXML = strXML + "/></ROOT> ";
                return strXML;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet fmCurrency_Get_Info(string strUserID, string strCurrency_ID)
        {
            try
            {
                DataSet dsTmp;
                string strQuery;
                strQuery = PrepareXML(T_BOS_CURRENCY, "100", "Admin", "Admin", strUserID, "G", strCurrency_ID);


                dsTmp = Fetch_DataSet(strQuery);

                return dsTmp;

                dsTmp = null/* TODO Change to default(_) if this is not a reference type */;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private IMiddlewareService TpService()
        {
            Uri uri = new Uri("http://10.157.13.69:21012");

            var enconding = new WcfCoreMtomEncoder.MtomMessageEncoderBindingElement(new TextMessageEncodingBindingElement(MessageVersion.Soap11, System.Text.Encoding.UTF8));
            var transport = new HttpTransportBindingElement();
            transport.TransferMode = TransferMode.Buffered;

            var binding = new CustomBinding(enconding, transport);


            EndpointAddress endpoint = new EndpointAddress("http://10.1.46.45/MiddlewareService.svc");
            ChannelFactory<IMiddlewareService> channelFactory = new ChannelFactory<IMiddlewareService>(binding, endpoint);
            var webservice = channelFactory.CreateChannel();
            return webservice;

        }
        private DataSet Fetch_DataSet(string strQuery)
        {
            DataSet dtReturn = new DataSet();
            DataSet dtResult = new DataSet();

            try
            {

                IMiddlewareService tp = TpService();

                XmlTextReader reader = null;
                SendRequest sr = new SendRequest(Convert.ToBase64String(zip(strQuery)));
                SendResponse strXMLResult = tp.Send(sr);
                var srXML = new StringReader(unzip(Convert.FromBase64String(strXMLResult.SendResult)));

                reader = new XmlTextReader(srXML);
                //// XML to DataSet
                dtReturn.ReadXml(reader);

                //strXMLResult = newTP.Send(Convert.ToBase64String(zip(strQuery)))
                //Debug.Print(unzip(Convert.FromBase64String(Convert.ToBase64String(zip(strQuery)))))
                //strXMLResult = unzip(Convert.FromBase64String(strXMLResult))
                // ----------------------------------------------------

                if (dtReturn.Tables.Count > 0)
                {
                    if (dtReturn.Tables[0].Rows.Count == 0)
                        dtReturn.Tables.Clear();
                }
                // ----------------------------------------------------
                return dtReturn;
            }
            catch (Exception ex)
            {
                //if (strXMLResult. .Length > 0)
                //    throw new InvalidOperationException(strXMLResult.ToString());
                //else
                throw (ex);
            }


        }

        public string unzip(byte[] bytes)
        {
            using (MemoryStream msi = new MemoryStream(bytes))
            {
                using (MemoryStream mso = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        CopyTo(gs, mso);
                    }
                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
        }
        public byte[] zip(String str)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(str);
            using (MemoryStream msi = new MemoryStream(Bytes))
            {
                using (MemoryStream mso = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        CopyTo(msi, gs);
                    }
                    return mso.ToArray();
                }
            }
        }

        public void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt = src.Read(bytes, 0, bytes.Length);
            while ((cnt != 0))
            {
                dest.Write(bytes, 0, cnt);
                cnt = src.Read(bytes, 0, bytes.Length);
            }
        }
    }
}
