using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebdriverFramework.Framework.util;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    public class Table : BaseElement
    {
        private String locTableRows = ".//tbody/tr";
        private String locTableColumns = ".//td";
        private IList<IWebElement> rows;
        private IList<IWebElement> columns;
        private By locator;

        public Table(By locator, String name) : base(locator, name)
        {
            this.locator = locator;
        }

        private void FillElements()
        {
            int i = 0;
            while (i < 5)
            {
                try
                {
                    IWebElement table_element = Browser.GetDriver().FindElement(locator);
                    rows = table_element.FindElements(By.XPath(locTableRows));
                    columns = rows[0].FindElements(By.XPath(locTableColumns));
                    break;
                }
                catch (Exception e)
                {
                    i++;
                }
            }
        }

        protected override string GetElementType()
        {
            return "loc.tab";
        }

        public String GetCell(int row, int column)
        {
            FillElements();
            IWebElement cell = rows[row].FindElements(By.XPath(locTableColumns))[column];
            return cell.Text;
        }

        public IWebElement GetCellElement(int row, int column)
        {
            FillElements();
            IWebElement cell = rows[row].FindElements(By.XPath(locTableColumns))[column];
            return cell;
        }

        public void ClickCell(int row, int column)
        {
            int i = 0;
            FillElements();
            IWebElement cell = rows[row].FindElements(By.XPath(locTableColumns))[column];
            while (i < 5)
            {
                try
                {
                    cell.Click();
                    System.Threading.Thread.Sleep(200);
                    break;
                }
                catch (Exception e) { i++; }
            }
        }

        public int GetRowsCount()
        {
            FillElements();
            return rows.Count;
        }

        public int GetColumnsCount()
        {
            FillElements();
            return columns.Count;
        }

        public Link FindElementWithTxt(string elementText)
        {
            foreach (var row in rows)
            {
                if (columns.Count != 0)
                    foreach (var column in columns)
                    {
                        if (column.Text == elementText)
                        {
                            return new Link(By.LinkText(column.Text), "Element from list find");
                        }
                    }
            }
            return null;
        }
    }
}


