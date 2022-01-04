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

        public PairsDomain PairsDomain { get; init; }

        public StudentsDomain StudentsDomain { get; init; }

        readonly SendEmail _emailSender;

        readonly IConfiguration _config;

        public DomainsContainer(IConfiguration config, SendEmail emailSender)
        {
            _config = config;
            _repositories = new RepositoriesContainer(_config);
            PairsDomain = new PairsDomain(_repositories.PairRepositry);
            StudentsDomain = new StudentsDomain(_repositories.StudentRepositry);
            _emailSender = emailSender;
        }

        public async Task TestEmailToOne()
        {
            var student = await _repositories
                .StudentRepositry.GetByIdAsync(3);
            
            await _emailSender.To(new List<string>{ student.Email })
                .SendAutoEmailAsync(student, Templates.SuccessfullyRegisteredHebrew);

        }

        public async Task TestEmailToMany()
        {
            var students = await _repositories
                .StudentRepositry.GetAllAsync();

            var emails = from s in students
                         select s.Email;

            await _emailSender.To(emails)
                .Subject("Test many: subject")
                .Template(new StringBuilder()
                .Append("Test many: body"))
                .SendOpenMailAsync(new List<string> 
                { 
                    @"C:\Users\Asuspcc\Desktop\מכון לב\מסמכים\appendix11702.pdf" 
                });
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

            await StudentsDomain.InsertManyStudents(parser.NewStudents);

            await _repositories.ConfigRepositry.SaveSpredsheetLastRange(spredsheetLastRange);
        }
    }
}
