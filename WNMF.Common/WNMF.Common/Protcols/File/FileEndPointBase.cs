/***************************************************************
 * Notice:
 *       1) Do not remove copyright notice
 *       2) See License file (https://raw.githubusercontent.com/dx-prog/WildNetworkMessagingFramework/master/LICENSE) for more details 
 *       3) Copyright (c) 2017 David Garcia
 * ************************************************************/
 using System;
using System.IO;
using WNMF.Common.Culture;
using WNMF.Common.Definition;
using WNMF.Common.Foundation;

namespace WNMF.Common.Protcols.File {
    public class FileEndPointBase : NetworkEndPointBase {
        public const string TypeDefinition = ".TYPE";
        public const string IgnoreDefinition = ".IGNORE";

        protected FileEndPointBase(Uri uri) : base(uri) {
        }

        public static string GetRandomFileName() {
            return Path.GetRandomFileName().Replace(".", "") + "." + DateTime.Now.Ticks + ".dat";
        }

        public static bool TryGetDescription(string filePath,
            out TryOperationResponse<NetworkMessageDescription> description) {
            description = null;
            try {
                var extension = Path.GetExtension(filePath).ToUpperInvariant();

                switch (extension) {
                    case IgnoreDefinition:
                        return false;
                    case TypeDefinition:
                        return false;
                }

                // a File can be marked as ignored
                if (System.IO.File.Exists(filePath + IgnoreDefinition))
                    return false;
                // Files without a type definition are automatically ignored
                if (!System.IO.File.Exists(filePath + TypeDefinition))
                    return false;

                var fi = new FileInfo(filePath);
                var desc = new NetworkMessageDescription(fi, System.IO.File.ReadAllText(filePath + TypeDefinition));
                description = new TryOperationResponse<NetworkMessageDescription>(
                    LocalizationKeys.ForGeneralPurposes.Success,
                    desc);

                return true;
            }
            catch (Exception ex) {
                description =
                    new TryOperationResponse<NetworkMessageDescription>(ex,
                        LocalizationKeys.ForGeneralPurposes.UnclassifiedError);
                return false;
            }
            finally {
                if (description == null)
                    description =
                        new TryOperationResponse<NetworkMessageDescription>(
                            LocalizationKeys.NetworkMessageHandler.MessageNotFound,
                            null);
            }
        }
    }
}