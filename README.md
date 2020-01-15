# AlphaVantageRetriever

This is a quick-and-dirty C# app I put together to download a year's worth of stock price data from AlphaVantage. I needed to do that
to comply with California's Form 700 annual filing requirements for public officials but it can be used by anyone who wants to grab
AlphaVantage data. It utilizes an Sqlite3 database to store the price data it retrieves.

All options can be set from the command line or in `configInfo.json`. Here's what you'll see if you run the app as `AlphaVantageRetriever --help`:

```
an app to retrieve stock ticker prices from AlphaVantage and export them in a tabular format

Usage: AlphaVantageRetriever [options]

Options:
  -x|--export          export data from database to CSV file (default is '<reporting year|current year> Pricing Data.csv'
  -u|--update          Update securities in database from CSV file (default is 'Securities Data.csv')
  -g|--get             get data from AlphaVantage
  -r|--replace         replace existing data (only applies to -g|--get and -u|--update)
  -y|--year            year (YYYY) to store (only applies to -g|--get)
  -c|--calls           calls per minute to AlphaVantage site (only applies to -g|--get)
  -k|--key             your AlphaVantage API key
  -?|-h|--help         Show help information
```
  
The three activities supported are:
  
* -x|--export
   
   Exports data that you've previously retrieved from AlphaVantage to a CSV file. You can add an optional file path or
   accept the default.
     
* -u|--update
   
   Updates the list of securities in the database whose pricing information will be retrieved from AlphaVantage. You
   can specify a file path or accept the default.
   
   The file format must include the properties defined in `ImportedSecurityInfo.cs` or you must rewrite that file.
  
   * -r|--replace
      
      if specified, information on securities already in the database will be updated (i.e., not specifying the option
      only adds new securities (based on CUSIP id) to the database).
      
* -g|--get
   
   Retrieves data from AlphaVantage. 
   
   You must specify a reporting year, either with the -y|--year option or in the configuration file. 
   
   If you don't specify how many queries against AlphaVantage to make per minute via -c|--callsperminute or in the configuration 
   file it defaults to 4.5 (unpaid access to AlphaVantage is limited to 5 queries per minute and 500 per day).
   
   You must specify your AlphaVantage API key either via the -k|--key option, the configuration file (not a good security
   practice) or in your app's user secret store (generally only useful when running within Visual Studio).
   
   * -y|--year
      
      must be >= 2000 and less than the current year
      
   * -c|--callsperminute
      
      must be greater than 0. If you don't have a paid AlphaVantage account setting this value above 5 will lead to some
      retrievals being rejected.
      
   * -r|--replace
      
      if specified, securities whose price data is already in the database will be retrieved again from AlphaVantage (i.e, not
      specifying the option skips any securities with pricing data already in the database).
      
   * -k|--key
      
      your AlphaVantage API key
