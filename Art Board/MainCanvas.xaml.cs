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
using System.Windows.Shapes;

namespace Art_Board
{
    public partial class MainCanvas : Window
    {
        private List<Image> images = new List<Image>();
        public MainCanvas()
        {
            InitializeComponent();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter =
                "Image Files (*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";

            if ((bool)dialog.ShowDialog())
            {
                var bitmap = new BitmapImage(new Uri(dialog.FileName));
                var image = new Image { Source = bitmap };
                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                images.Add(image);
                image.Width = image.Source.Width;
                image.Height = image.Source.Height;
                canvas.Children.Add(image);
            }
        }

        private Image draggedImage;
        private Point mousePosition;

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;

            if (image != null && canvas.CaptureMouse())
            {
                mousePosition = e.GetPosition(canvas);
                draggedImage = image;
                Panel.SetZIndex(draggedImage, 1); // in case of multiple images
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null)
            {
                canvas.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 0);
                draggedImage = null;
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    Image image = canvas.Children[i] as Image;
                    if (image != null)
                    {
                        var position = e.GetPosition(canvas);
                        var offset = position - mousePosition;
                        Canvas.SetLeft(image, Canvas.GetLeft(image) + offset.X);
                        Canvas.SetTop(image, Canvas.GetTop(image) + offset.Y);
                    }
                }
            }
            else if (draggedImage != null)
            {
                var position = e.GetPosition(canvas);
                var offset = position - mousePosition;
                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
            }
            mousePosition = e.GetPosition(canvas);
        }

        bool isTransparent = false;
        private void CanvasToggleTransparent(object sender, RoutedEventArgs e)
        {
            if (isTransparent)
            {
                Brush filled = new SolidColorBrush(Colors.DimGray);
                filled.Opacity = 0.25;
                this.Background = filled;

            }
            else
            {
                Brush transparent = Brushes.Transparent;
                this.Background = transparent;
            }
            isTransparent = !isTransparent;
        }

        bool alwaysOnTop = false;
        private void SetWindowOnTop(object sender, RoutedEventArgs e)
        {
            alwaysOnTop = !alwaysOnTop;
        }

        private void WindowDeactivated(object sender, EventArgs e)
        {
            if (alwaysOnTop)
            {
                Window window = (Window)sender;
                window.Topmost = true;
            }
        }

        private void CanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            for (int i = 0; i < images.Count; i++)
            {
                if (e.Delta < 0)
                {
                    images[i].Width = images[i].Width / 2;
                    images[i].Height = images[i].Height / 2;
                } else
                {
                    images[i].Width = images[i].Width * 2;
                    images[i].Height = images[i].Height * 2;
                }
                Console.WriteLine(images[i].Width);
                Console.WriteLine(images[i].Height);
            }
        }
    }
}
