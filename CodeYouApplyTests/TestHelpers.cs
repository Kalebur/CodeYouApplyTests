﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace ApplicationPageTests
{
    public class TestHelpers
    {
        private readonly Random _random = new();

        /// <summary>
        /// Select's a random element from a given collection of elements.
        /// </summary>
        /// <param name="elements">A collection of elements to select an item in.</param>
        /// <param name="maxIndex">The exclusive upperbound index for possible choices. If no value is provided, collection's Count property will be used. </param>
        /// <param name="minIndex">The minimum index a choice can be selected from.</param>
        public void SelectRandomElementInCollection(IList<IWebElement> elements, int? maxIndex = null, int minIndex = 0)
        {
            maxIndex ??= elements.Count;
            var randomIndex = _random.Next(minIndex, (int)maxIndex);

            elements[randomIndex].ClickViaJavaScript();
        }

        public static int GetSelectedItemsCount(IList<IWebElement> elements)
        {
            return elements.Where(element => element.Selected).Count();
        }

        public DateTime GetRandomBirthdate(BirthdateRange rangeType)
        {
            int month = _random.Next(1, 13);
            int day;

            if (month == 2)
            {
                day = _random.Next(1, 29);
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                day = _random.Next(1, 31);
            }
            else
            {
                day = _random.Next(1, 32);
            }

            var year = rangeType switch
            {
                BirthdateRange.Future => DateTime.Today.AddYears(_random.Next(1, 100)).Year,
                BirthdateRange.Under18 => DateTime.Today.AddYears(-_random.Next(0, 18)).Year,
                _ => DateTime.Today.AddYears(-_random.Next(18, 100)).Year,
            };
            return new DateTime(year, month, day);
        }

        public List<string> GetSelectOptionsAsStrings(SelectElement selectElement)
        {
            List<string> optionsAsStrings = [];

            foreach (var option in selectElement.Options)
            {
                optionsAsStrings.Add(option.Text);
            }

            return optionsAsStrings;
        }
    }
}
