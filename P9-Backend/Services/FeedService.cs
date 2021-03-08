using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace P9_Backend.Services
{
    public class FeedService : IFeedService
    {
        //FileType of videos
        private string fileType = ".mp4";

        //Predictive videos with a person
        private string[] _pVids = 
        { 
            "Drone1Pout",
            "Drone2Pout",
            "Drone3Pout",
            "Drone4Pout",
            "Drone5Pout",
            "Drone6Pout",
            "Drone7Pout",
            "Drone8Pout",
        };

        //Non-Predictive videos with a person
        private string[] _npVids =
        {
            "Drone1P",
            "Drone2P",
            "Drone3P",
            "Drone4P",
            "Drone5P",
            "Drone6P",
            "Drone7P",
            "Drone8P",
        };

        //Non-Predictive videos without a person
        private string[] _cVids =
        {
            "Drone1",
            "Drone2",
            "Drone3",
            "Drone4",
            "Drone5",
            "Drone6",
            "Drone7",
            "Drone8",
        };

        /// <summary>
        /// Generates a list of feed URLs.
        /// </summary>
        /// <param name="predictive">Decides if feeds should contain a predictive video</param>
        /// <param name="host">the host of the request</param>
        /// <returns>An Enumerable of string containing the feed URLs</returns>
        public async Task<ActionResult<IEnumerable<string>>> GetFeeds(bool predictive, string host)
        {
            List<string> feedsList = new List<string>();
            Random rand = new Random();
            int SelectedPersonVideo = rand.Next(_pVids.Length);
            string baseurl = host + "/videos/";


            await Task.Run(() => {
                if (predictive)
                {
                    feedsList.Add(baseurl + _pVids[SelectedPersonVideo] + fileType);
                }
                else
                {
                    feedsList.Add(baseurl + _npVids[SelectedPersonVideo] + fileType);
                }

                for (int i = 0; i < _cVids.Length; i++)
                {
                    if (i != SelectedPersonVideo)
                    {
                        feedsList.Add(baseurl + _cVids[i] + fileType);
                    }
                }

                feedsList = feedsList.OrderBy(v => rand.Next()).ToList();
            });

            return feedsList;
        }
    }
}
