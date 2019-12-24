using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Sanet.SmartSkating.WearOs.Views.Components
{
    public class TitledLabelView:LinearLayout
    {
        private TextView _titleText;
        private TextView _valueText;

        protected TitledLabelView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize ();
        }

        public TitledLabelView(Context context) : base(context)
        {
            Initialize ();
        }

        public TitledLabelView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize ();
        }

        public TitledLabelView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize ();
        }

        public TitledLabelView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize ();
        }
        
        void Initialize ()
        {
            this.Orientation = Orientation.Vertical;
            _titleText = new TextView(Context)
            {
                TextSize = 14
            };
            
            _valueText = new TextView(Context)
            {
                TextSize = 18
            };

            AddView (_titleText);
            AddView(_valueText);
        }

        public string TitleText
        {
            get => _titleText.Text;
            set => _titleText.Text = value;
        }
        
        public string ValueText
        {
            get => _valueText.Text;
            set => _valueText.Text = value;
        }
    }
}