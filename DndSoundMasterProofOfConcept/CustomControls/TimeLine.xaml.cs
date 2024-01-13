using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DndSoundMasterProofOfConcept.CustomControls
{
    /// <summary>
    /// Interaction logic for TimeLine.xaml
    /// </summary>
    public partial class TimeLine : UserControl
    {
        int spacing;
        public TimeLine()
        {
            InitializeComponent();
        }

        public void InitiateTimeLabels(int spacing, TimeSpan totalTime)
        {
            double totalTimeInSeconds = totalTime.TotalSeconds;
            
            this.spacing = spacing;
            for (int i = 0; i <= spacing; i++) 
            {
                Label label = new Label() { Content = string.Format("{0:mm\\:ss}", TimeSpan.FromSeconds(totalTimeInSeconds) - TimeSpan.FromSeconds(i * (totalTimeInSeconds / spacing)))
            };
                if(i == 0)
                    label.Margin = new Thickness(Width - (i * (Width / spacing) + 60), 20, 0, 0);
                else if(i == spacing)
                    label.Margin = new Thickness(0, 20, 0, 0);
                else
                    label.Margin = new Thickness(Width - (i * (Width / spacing)), 20, 0, 0);


                grid.Children.Add(label);
            }
        }
        
        public void ResizeLabels(double width)
        {
            Width = line.X2 = width;

            int i = 0;
            foreach(UIElement label in grid.Children) 
            {
                if(label is Label) 
                {
                    Label labelCopy = label as Label;
                    if (i == 0)
                        labelCopy.Margin = new Thickness(Width - (i * (Width / spacing) + 60), 20, 0, 0);
                    else if (i == spacing)
                        labelCopy.Margin = new Thickness(0, 20, 0, 0);
                    else
                        labelCopy.Margin = new Thickness(Width - (i * (Width / spacing)), 20, 0, 0);
                    i++;
                }
            }
        }
    }
}
