# NorthwindConsole


This project will not be able to successfully connect to a database without an <i>appsettings.json</i>, make sure to add one.

As this file will contain sensitive information (user names and passwords) it is <b>NOT</b> included.

A generic form for such a file should look like this:

<pre>
<code>
{"NorthwindContext": {"ConnectionString": "Server=bitsql.wctc.edu;Database=NorthWind_88_EH;User ID=****;Password=****"}}
</code>
</pre>

<b>PLEASE</b> Remember to replace the asterisks with your own appropriate name and password.
