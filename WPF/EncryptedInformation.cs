using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{
    public class EncryptedInformation
    {
        private string id;
        private string name;
        private string key;
        private string encryptedData;

        public string Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Key { get { return key; } set { key = value; } }
        public string EncryptedData { get { return encryptedData; } set { encryptedData = value; } }
    }
}
