# AlphaVantageRetriever

This is a quick-and-dirty C# app I put together to download a year's worth of stock price data from AlphaVantage. I needed to do that
to comply with California's Form 700 annual filing requirements for public officials but it can be used by anyone who wants to grab
AlphaVantage data.

The available command options are:

```
an app to retrieve stock ticker prices from AlphaVantage and export them in a tabular format

Usage: AlphaVantageRetriever [options]

Options:
  -x|--export          export data from database to CSV file (default is '<reporting year> Pricing Data.csv'
  -u|--update          Update securities in database from CSV file (default is 'Securities Data.csv')
  -g|--get             get data from AlphaVantage
  -r|--replace         replace existing data (only applies to -g|--get and -u|--update)
  -y|--year            year (YYYY) to store (only applies to -g|--get)
  -c|--CallsPerMinute  calls per minute to AlphaVantage site (only applies to -g|--get)
  -?|-h|--help         Show help information
  ```
  
