/*
 * Copyright (c) James Birchall 2013
*/ 

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
using System.Drawing;
using jbirchall_shuffle;

namespace jbirchall_pandell_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int sn_range = 10000;

        private WriteableBitmap m_sortedBuffer;
        private WriteableBitmap m_shuffledBuffer;
        private int[] m_sortedArray = null;
        private int[] m_shuffledArray = null;

        private Int32Rect m_rect;
        private int m_bpp;
        private int m_stride;
        private int m_arraySize;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Created by default automatically.
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();


            int width = (int)Math.Ceiling(Math.Sqrt(sn_range));
            int height = width;

            m_shuffledBuffer = new WriteableBitmap(width, height, 96, 96, PixelFormats.Cmyk32, null);
            m_sortedBuffer = new WriteableBitmap(width, height, 96, 96, PixelFormats.Cmyk32, null);
            m_rect = new Int32Rect(0, 0, m_sortedBuffer.PixelWidth, m_sortedBuffer.PixelHeight);
            m_bpp = (m_sortedBuffer.Format.BitsPerPixel + 7) / 8;
            m_stride = m_sortedBuffer.PixelWidth * m_bpp;
            m_arraySize = m_stride * m_sortedBuffer.PixelHeight;

            imgSorted.Source = m_sortedBuffer;
            imgShuffled.Source = m_shuffledBuffer;

            imgShuffled.Width = width;
            imgShuffled.Height = height;

            imgSorted.Width = width;
            imgSorted.Height = height;

            txtOutput.IsReadOnly = true;
        }

        /// <summary>
        /// Executed when the "Shuffle" button is clicked.  Changes the destination image based on the 
        /// contents of the shuffled array.  It's easiest to see whether something is actually random 
        /// (and not just different) by visualising the output.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShuffle_OnClick(object sender, RoutedEventArgs e)
        {
            Shuffler.FisherYatesShuffle<int>(m_shuffledArray);

            displayShuffleData();
        }

        /// <summary>
        /// Shows the contents of the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainFrm_AfterLoad(object sender, RoutedEventArgs e)
        {
            m_sortedArray = new int[sn_range];
            for (int i = 0; i < sn_range; i++)
                m_sortedArray[i] = i + 1;

            m_sortedBuffer.WritePixels(m_rect, m_sortedArray, m_stride, 0);
            imgSorted.Source = m_sortedBuffer;

            m_shuffledArray = Shuffler.DurstenfeldShuffle<int>(sn_range, IntGenerator.LinearSequence());

            displayShuffleData();
        
        }

        /// <summary>
        /// Resets everything to the sorted state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_OnClick(object sender, RoutedEventArgs e)
        {
            Array.Sort<int>(m_shuffledArray);
            displayShuffleData();
        }

        /// <summary>
        /// Convenience function to visualise the contents of the shuffled array.
        /// </summary>
        private void displayShuffleData(){
            // Update the image.
            m_shuffledBuffer.WritePixels(m_rect, m_shuffledArray, m_stride, 0);
            imgShuffled.Source = m_shuffledBuffer;

            // Update the text box.
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_shuffledArray.Length; i++)
            {
                sb.AppendFormat("{0} ", m_shuffledArray[i].ToString("00000"));
            }
            txtOutput.Text = sb.ToString();
        }

    }
}
