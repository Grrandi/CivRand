﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CivilizationRand
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : CivilizationRand.Common.LayoutAwarePage
    {
        //Bool to get what type of list to create for user to choose civilizations from
        private bool gNK = false;
        private bool GNK
        {
            get { return gNK; }
            set { gNK = value; }
        }

        // If dlc civs to be added to the list or not
        private bool dlc = false;
        private bool DLC
        {
            get { return dlc; }
            set { dlc = value; }
        }

        // How many players to select for
        private int players = 1;
        private int PLAYERS
        {
            get { return players; }
            set
            {
                if (value >= 1 && value <= 12)
                    players = value;
                txtBoxPpl.Text = "" + players;
            }
        }

        // How many civs per player to choose from
        private int drawnCiv = 1;
        private int DRAWNCIV
        {
            get { return drawnCiv; }
            set
            {
                if (value >= 1 && value <= 4)
                    drawnCiv = value;
                txtBoxCivRand.Text = "" + drawnCiv;
            }
        }

        // Creating arrays to hold civilization strings
        private static String[] basicCivs = {"America","Arabia","Aztecs","China","Egypt","England","France","Germany","Greece","India","Iroquois","Japan","Ottomans","Persia","Rome","Russia","Siam","Songhai"};
        private static String[] GnKCivs = { "Austria", "Byzantium", "Carthage", "Celts", "Ethiopia", "Huns", "Maya", "Netherlands", "Sweden" };
        private static String[] DLCcivs = { "Babylon", "Denmark", "Inca", "Korea", "Mongolia", "Polynesia", "Spain" };
        private List<String> choosableCivs = new List<string>();

        public MainPage()
        {            
            this.InitializeComponent();
            updateCivList();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Updates the choosableCiv list to correspond user's options wether to include Gods & Kings expansion and/or DLC civilizations or not
        /// </summary>
        private void updateCivList()
        {
            //Empty the list before adding anything
            choosableCivs.Clear();

            //Empty the listbox that you see in the screen
            civListBox.Items.Clear();

            foreach (String str in basicCivs)
            {
                choosableCivs.Add(str);
            }

            if (GNK == true)
            {
                foreach (String str in GnKCivs)
                {
                    choosableCivs.Add(str);
                }
            }

            if (DLC == true)
            {
                foreach (String str in DLCcivs)
                {
                    choosableCivs.Add(str);
                }
            }

            // Add every item in choosableCivs list to screen
            foreach (String str in choosableCivs)
            {
                CheckBox item = new CheckBox();
                item.Content = str;
                civListBox.Items.Add(item);
            }
        }

        private void switchGnK_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch wup = (ToggleSwitch)e.OriginalSource;
            GNK = wup.IsOn;
            updateCivList();
        }

        private void switchDLC_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch wup = (ToggleSwitch)e.OriginalSource;            
            DLC = wup.IsOn;
            updateCivList();
        }

        private void btnSelectAll_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in civListBox.Items)
            {
                box.IsChecked = true;
            }
        }

        private void btnRemoveAll_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in civListBox.Items)
            {
                box.IsChecked = false;
            }
        }        

        private void btnGo_Click_1(object sender, RoutedEventArgs e)
        {

            // Pick selected civs from the UI and store them in a list
            List<String> chosenCivs = createChosen();            
            // Shuffles the chosen civs to a random order
            shuffler(chosenCivs);

            // For formating the data
            List<RandomCreatureCollection> collection = new List<RandomCreatureCollection>();

            for (int i = 0; i < PLAYERS; i++)
            {

                List<String> tempCivs = chosenCivs.ToList();

                // Placeholder object
                RandomCreatureCollection hurr = new RandomCreatureCollection();
                hurr.PlayerNumber = "Player " + (i + 1);                

                for (int j = DRAWNCIV; j > 0; j--)
                {
                    if (chosenCivs.Count == 0)
                    {
                        chosenCivs = createChosen();
                        shuffler(chosenCivs);
                    }

                    if (chosenCivs.Count != 0)
                    {
                        hurr.Civs.Add(chosenCivs[chosenCivs.Count - 1]);
                        chosenCivs.RemoveAt(chosenCivs.Count - 1);
                    }

                }

                collection.Add(hurr);
            }

            randomizedCivs.ItemsSource = collection;
        }

        /// <summary>
        /// Creates a list from ticked checkboxes from the UI
        /// </summary>
        /// <returns>A list that contais only the names of ticked civilizations</returns>
        private List<String> createChosen()
        {
            List<String> chosenCivs = new List<string>();

            List<Object> tickedCivs = civListBox.Items.ToList();
            CheckBox cBox;
            foreach (Object obj in tickedCivs)
            {
                cBox = (CheckBox)obj;
                if (cBox.IsChecked == true)
                {
                    chosenCivs.Add(cBox.Content.ToString());
                }
            }

            return chosenCivs;
        }

        /// <summary>
        /// Randomizes the order of chosen civilizations in the list
        /// </summary>
        /// <param name="list">The list that is to be shuffled</param>
        private void shuffler(IList<String> list)
        {
            Random rnd = new Random();

            int n = list.Count;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                String temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        private void btnPplInc_Click_1(object sender, RoutedEventArgs e)
        {
            PLAYERS++;
        }

        private void btnPplDec_Click_1(object sender, RoutedEventArgs e)
        {
            PLAYERS--;
        }

        private void btnCivInc_Click_1(object sender, RoutedEventArgs e)
        {
            DRAWNCIV++;
        }

        private void btnCivDec_Click_1(object sender, RoutedEventArgs e)
        {
            DRAWNCIV--;
        }

        private void randomizedCivs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            randomizedCivs.SelectedIndex = -1;
        }

    }
}
