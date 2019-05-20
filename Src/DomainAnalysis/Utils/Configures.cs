using DomainAnalysis.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.Utils
{
    public class Configures
    {
        #region original string
        public static string GetAutoRawMaterialFileDir()
        {
            string value = Settings.Default[Constants.KeyAutoRawMaterialFileDir].ToString();
            return value;
        }

        public static void SaveAutoRawMaterialFileDir(string value)
        {
            Settings.Default[Constants.KeyAutoRawMaterialFileDir] = value;
            Settings.Default.Save();
        }


        public static string GetManualSearchTermPath()
        {
            string value = Settings.Default[Constants.keyManualExtraSearchTermsPath].ToString();
            return value;
        }

        public static void SaveManualSearchTermPath(string value)
        {
            Settings.Default[Constants.keyManualExtraSearchTermsPath] = value;
            Settings.Default.Save();
        }

        public static bool GetAutoIsDeleteExistingFile()
        {
            bool value = (bool)Settings.Default[Constants.KeyAutoIsDeleteExistingFile];
            return value;
        }

        public static void SaveAutoIsDeleteExistingFile(bool value)
        {
            Settings.Default[Constants.KeyAutoIsDeleteExistingFile] = value;
            Settings.Default.Save();
        }

        public static bool GetAutoWizardIsDeleteExistingFile()
        {
            bool value = (bool)Settings.Default[Constants.KeyAutoWizardIsDeleteExistingFile];
            return value;
        }

        public static void SaveAutoWizardIsDeleteExistingFile(bool value)
        {
            Settings.Default[Constants.KeyAutoWizardIsDeleteExistingFile] = value;
            Settings.Default.Save();
        }

        public static string GetAutoWizardTopicNumberArray()
        {
            string value = Settings.Default[Constants.KeyAutoWizardTopicNumberArray].ToString();
            return value;
        }

        public static void SaveAutoWizardTopicNumberArray(string value)
        {
            Settings.Default[Constants.KeyAutoWizardTopicNumberArray] = value;
            Settings.Default.Save();
        }

        public static string GetAutoWizardMaxIteration()
        {
            string value = Settings.Default[Constants.KeyAutoWizardMaxIteration].ToString();
            return value;
        }

        public static void SaveAutoWizardMaxIteration(string value)
        {
            Settings.Default[Constants.KeyAutoWizardMaxIteration] = value;
            Settings.Default.Save();
        }

        public static string GetAutoWizardRawFilePath()
        {
            string value = Settings.Default[Constants.KeyAutoWizardRawFilePath].ToString();
            return value;
        }

        public static void SaveAutoWizardRawFilePath(string value)
        {
            Settings.Default[Constants.KeyAutoWizardRawFilePath] = value;
            Settings.Default.Save();
        }

        public static string GetManualRawMaterialFileDir()
        {
            string value = Settings.Default[Constants.KeyManualRawMaterialFileDir].ToString();
            return value;
        }

        public static void SaveManualRawMaterialFileDir(string value)
        {
            Settings.Default[Constants.KeyManualRawMaterialFileDir] = value;
            Settings.Default.Save();
        }

        public static string GetManualModelFilePath()
        {
            string value = Settings.Default[Constants.KeyManualModelFilePath].ToString();
            return value;
        }

        public static void SaveManualModelFilePath(string value)
        {
            Settings.Default[Constants.KeyManualModelFilePath] = value;
            Settings.Default.Save();
        }

        public static bool GetManualIsDeleteExistingFile()
        {
            bool value = (bool)Settings.Default[Constants.KeyManualIsDeleteExistingFile];
            return value;
        }

        public static void SaveManualIsDeleteExistingFile(bool value)
        {
            Settings.Default[Constants.KeyManualIsDeleteExistingFile] = value;
            Settings.Default.Save();
        }

        public static bool GetManualWizardIsDeleteExistingFile()
        {
            bool value = (bool)Settings.Default[Constants.KeyManualWizardIsDeleteExistingFile];
            return value;
        }

        public static void SaveManualWizardIsDeleteExistingFile(bool value)
        {
            Settings.Default[Constants.KeyManualWizardIsDeleteExistingFile] = value;
            Settings.Default.Save();
        }

        public static string GetManualWizardModelFilePath()
        {
            string value = Settings.Default[Constants.KeyManualWizardModelFilePath].ToString();
            return value;
        }

        public static void SaveManualWizardModelFilePath(string value)
        {
            Settings.Default[Constants.KeyManualWizardModelFilePath] = value;
            Settings.Default.Save();
        }

        public static string GetManualWizardRawFilePath()
        {
            string value = Settings.Default[Constants.KeyManualWizardRawFilePath].ToString();
            return value;
        }

        public static void SaveManualWizardRawFilePath(string value)
        {
            Settings.Default[Constants.KeyManualWizardRawFilePath] = value;
            Settings.Default.Save();
        }
        #endregion
    }
}
