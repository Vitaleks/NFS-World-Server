/*Файл был найден на просторах интернета и использован в проекте. Да простит меня автор этих строк :)*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalLauncher
{
    class ModifyRegistry
    {
            private bool showError = false;

            public bool ShowError
            {
                get { return showError; }
                set { showError = value; }
            }

            private string subKey = "SOFTWARE\\" + Application.ProductName.ToUpper();

            public string SubKey
            {
                get { return subKey; }
                set { subKey = value; }
            }

            private RegistryKey baseRegistryKey = Registry.LocalMachine;
            public RegistryKey BaseRegistryKey
            {
                get { return baseRegistryKey; }
                set { baseRegistryKey = value; }
            }

            public string Read(string KeyName)
            {
                // Opening the registry key
                RegistryKey rk = baseRegistryKey;
                // Open a subKey as read-only
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistrySubKey doesn't exist -> (null)
                if (sk1 == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        // If the RegistryKey exists I get its value
                        // or null is returned.
                        return (string)sk1.GetValue(KeyName);
                    }
                    catch (Exception e)
                    {
                        // AAAAAAAAAAARGH, an error!
                        ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                        return null;
                    }
                }
            }

            public bool Write(string KeyName, object Value)
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    // I have to use CreateSubKey 
                    // (create or open it if already exits), 
                    // 'cause OpenSubKey open a subKey as read-only
                    RegistryKey sk1 = rk.CreateSubKey(subKey);
                    // Save the value
                    sk1.SetValue(KeyName.ToUpper(), Value);

                    return true;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Writing registry " + KeyName.ToUpper());
                    return false;
                }
            }

            public bool DeleteKey(string KeyName)
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.CreateSubKey(subKey);
                    // If the RegistrySubKey doesn't exists -> (true)
                    if (sk1 == null)
                        return true;
                    else
                        sk1.DeleteValue(KeyName);

                    return true;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Deleting SubKey " + subKey);
                    return false;
                }
            }

            public bool DeleteSubKeyTree()
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.OpenSubKey(subKey);
                    // If the RegistryKey exists, I delete it
                    if (sk1 != null)
                        rk.DeleteSubKeyTree(subKey);

                    return true;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Deleting SubKey " + subKey);
                    return false;
                }
            }

            public int SubKeyCount()
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.OpenSubKey(subKey);
                    // If the RegistryKey exists...
                    if (sk1 != null)
                        return sk1.SubKeyCount;
                    else
                        return 0;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Retriving subkeys of " + subKey);
                    return 0;
                }
            }

            public int ValueCount()
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.OpenSubKey(subKey);
                    // If the RegistryKey exists...
                    if (sk1 != null)
                        return sk1.ValueCount;
                    else
                        return 0;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Retriving keys of " + subKey);
                    return 0;
                }
            }

            private void ShowErrorMessage(Exception e, string Title)
            {
                if (showError == true)
                    MessageBox.Show(e.Message,
                                    Title
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
            }
        
    }
}
