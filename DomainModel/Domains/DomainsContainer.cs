using Microsoft.Extensions.Configuration;
using PairMatching.DataAccess.Repositories;
using PairMatching.DomainModel.Email;
using PairMatching.DomainModel.GoogleSheet;
using PairMatching.Models;
using PairMatching.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.Domains
{
    public class DomainsContainer
    {
        readonly RepositoriesContainer _repositories;

        public PairsDomain PairsDomain { get; set; }

        readonly SendEmail _emailSender;

        public StudentsDomain StudentsDomain { get; set; }

        readonly IConfiguration _config;

        public DomainsContainer(IConfiguration config, SendEmail emailSender)
        {
            _config = config;
            _repositories = new RepositoriesContainer(_config);
            PairsDomain = new PairsDomain(_repositories.PairRepositry);
            StudentsDomain = new StudentsDomain(_repositories.StudentRepositry);
            _emailSender = emailSender;
        }

        public async Task Initialization()
        {
            var spredsheetLastRange = 
                await _repositories.ConfigRepositry.GetSpredsheetLastRange();

            var tempLastRange = spredsheetLastRange.Clone();

            // create parser for the spreadsheets
            GoogleSheetParser parser = new GoogleSheetParser();
            
            // read the English sheet
            var englishSheets = parser.ReadAsync(
                new EnglishDiscriptor(spredsheetLastRange, _config));
            
            // read the Hebrew sheet
            var hebrewSheets = parser.ReadAsync(
                new HebrewDescriptor(spredsheetLastRange, _config));


            await Task.WhenAll(englishSheets, hebrewSheets);

            spredsheetLastRange.EnglishSheets = englishSheets.Result;
            spredsheetLastRange.HebrewSheets = hebrewSheets.Result;

            // if there is no new data don't update the data base
            if (spredsheetLastRange.Equals(tempLastRange))
            {
                return;
            }

            var l = parser.NewStudents;
        }
    }
}
