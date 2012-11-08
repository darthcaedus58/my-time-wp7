// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.38967
//    <NameSpace>AppifierSchema</NameSpace><Collection>ObservableCollection</Collection><codeType>CSharp</codeType><EnableDataBinding>True</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>True</UseBaseClass><GenBaseClass>True</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net35</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace MyTime {
    using System.Collections.ObjectModel;
    using GalaSoft.MvvmLight;     
    
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.inputstudiowp7.com/schemas")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.inputstudiowp7.com/schemas", IsNullable=false)]
    public partial class Settings : ViewModelBase {
        
        private ObservableCollection<Setting> settingField;
        
        public Settings() {
            this.settingField = new ObservableCollection<Setting>();
        }
        
        [System.Xml.Serialization.XmlElementAttribute("Setting", Order=0)]
        public ObservableCollection<Setting> Setting {
            get {
                return this.settingField;
            }
            set {
                if ((this.settingField != null)) {
                    if ((settingField.Equals(value) != true)) {
                        this.settingField = value;
                        this.RaisePropertyChanged("Setting");
                    }
                }
                else {
                    this.settingField = value;
                    this.RaisePropertyChanged("Setting");
                }
            }
        }
    }
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.inputstudiowp7.com/schemas")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.inputstudiowp7.com/schemas", IsNullable=true)]
    public partial class Setting : ViewModelBase
    {
        
        private ObservableCollection<StringItem> stringItemField;
        
        private string nameField;
        
        private eDataType typeField;
        
        private string xamlField;
        
        private string valueField;
        
        public Setting() {
            this.stringItemField = new ObservableCollection<StringItem>();
        }
        
        [System.Xml.Serialization.XmlElementAttribute("StringItem", Order=0)]
        public ObservableCollection<StringItem> StringItem {
            get {
                return this.stringItemField;
            }
            set {
                if ((this.stringItemField != null)) {
                    if ((stringItemField.Equals(value) != true)) {
                        this.stringItemField = value;
                        this.RaisePropertyChanged("StringItem");
                    }
                }
                else {
                    this.stringItemField = value;
                    this.RaisePropertyChanged("StringItem");
                }
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                if ((this.nameField != null)) {
                    if ((nameField.Equals(value) != true)) {
                        this.nameField = value;
                        this.RaisePropertyChanged("Name");
                    }
                }
                else {
                    this.nameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public eDataType Type {
            get {
                return this.typeField;
            }
            set {
                if ((typeField.Equals(value) != true)) {
                    this.typeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Xaml {
            get {
                return this.xamlField;
            }
            set {
                if ((this.xamlField != null)) {
                    if ((xamlField.Equals(value) != true)) {
                        this.xamlField = value;
                        this.RaisePropertyChanged("Xaml");
                    }
                }
                else {
                    this.xamlField = value;
                    this.RaisePropertyChanged("Xaml");
                }
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                if ((this.valueField != null)) {
                    if ((valueField.Equals(value) != true)) {
                        this.valueField = value;
                        this.RaisePropertyChanged("Value");
                    }
                }
                else {
                    this.valueField = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }
    }
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.inputstudiowp7.com/schemas")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.inputstudiowp7.com/schemas", IsNullable=true)]
    public partial class StringItem : ViewModelBase
    {
        
        private string valueField;
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                if ((this.valueField != null)) {
                    if ((valueField.Equals(value) != true)) {
                        this.valueField = value;
                        this.RaisePropertyChanged("Value");
                    }
                }
                else {
                    this.valueField = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }
    }
    
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.inputstudiowp7.com/schemas")]
    public enum eDataType {
        
        /// <remarks/>
        Boolean,
        
        /// <remarks/>
        DateTime,
        
        /// <remarks/>
        Decimal,
        
        /// <remarks/>
        Integer,
        
        /// <remarks/>
        StringArray,
        
        /// <remarks/>
        String,

        /// <remarks/>
        Note,

        Header,

        ContactChooser,
    }
}