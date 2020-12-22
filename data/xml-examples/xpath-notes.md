# XPath example 1

The Xpath `/document/bookstore/child::*/child::title` which applyed to the document

	<?xml version="1.0" encoding="UTF-8"?>
	<document>
	  <bookstore name="Springler">
	    <book>
	      <title lang="en">Harry Potter</title>
	      <price>29.99</price>
	    </book>
	    <book>
	      <title lang="en">Learning XML</title>
	      <price>39.95</price>
	    </book>
	  </bookstore>
	  <bookstore name="Mir">
	    <book>
	      <title lang="ru">Война и Мир</title>
	      <price>19.99</price>
	    </book>
	    <book>
	      <title lang="ru">Тихй дон</title>
	      <price>19.95</price>
	    </book>
	  </bookstore>
	</document>


gets the result:

	<title lang="en">Harry Potter</title>
	<title lang="en">Learning XML</title>
	<title lang="ru">&#x412;&#x43E;&#x439;&#x43D;&#x430; &#x438; &#x41C;&#x438;&#x440;</title>
	<title lang="ru">&#x422;&#x438;&#x445;&#x439; &#x434;&#x43E;&#x43D;</title>


# XPath example 2

Task: select all title for the books in a category 'classic' from all bookstores

XPath: `/document/bookstore/child::book[@category='classic']/child::title`

Document:
``
	<?xml version="1.0" encoding="UTF-8"?>
	<document>
	<bookstore name="Springler">

	<book category="cooking">
	  <title lang="en">Everyday Italian</title>
	  <author>Giada De Laurentiis</author>
	  <year>2005</year>
	  <price>30.00</price>
	</book>

	<book category="children">
	  <title lang="en">Harry Potter</title>
	  <author>J K. Rowling</author>
	  <year>2005</year>
	  <price>29.99</price>
	</book>

	<book category="web">
	  <title lang="en">XQuery Kick Start</title>
	  <author>James McGovern</author>
	  <author>Per Bothner</author>
	  <author>Kurt Cagle</author>
	  <author>James Linn</author>
	  <author>Vaidyanathan Nagarajan</author>
	  <year>2003</year>
	  <price>49.99</price>
	</book>

	<book category="web">
	  <title lang="en">Learning XML</title>
	  <author>Erik T. Ray</author>
	  <year>2003</year>
	  <price>39.95</price>
	</book>
	</bookstore>

	<bookstore name="Mir">

	<book category="classic">
	  <title lang="ru">Война и Мир</title>
	  <author>Лев Толстой</author>
	  <price>19.99</price>
	</book>

	<book category="classic">
	  <title lang="ru">Тихй дон</title>
	  <author>Михаил Шолохов</author>
	  <price>19.95</price>
	</book>

	</bookstore>
	 
	</document>
``

Result:
``
	<title lang="ru">&#x412;&#x43E;&#x439;&#x43D;&#x430; &#x438; &#x41C;&#x438;&#x440;</title>
	<title lang="ru">&#x422;&#x438;&#x445;&#x439; &#x434;&#x43E;&#x43D;</title>
``

# Xpath example 3

Task: select nodes all 'bookstore' elements from a namespace `http://pum.donetsk.com/bookstore`
 
XPath: `/document/*[local-name()='bookstore' and namespace-uri()='http://pum.donetsk.com/bookstore']`

Document:
``
	<?xml version="1.0" encoding="UTF-8"?>
	<document>
	  <bookstore name="Springler">
	    <book category="cooking">
	      <title lang="en">Everyday Italian</title>
	      <author>Giada De Laurentiis</author>
	      <year>2005</year>
	      <price>30.00</price>
	    </book>
	    <book category="children">
	      <title lang="en">Harry Potter</title>
	      <author>J K. Rowling</author>
	      <year>2005</year>
	      <price>29.99</price>
	    </book>
	    <book category="web">
	      <title lang="en">XQuery Kick Start</title>
	      <author>James McGovern</author>
	      <author>Per Bothner</author>
	      <author>Kurt Cagle</author>
	      <author>James Linn</author>
	      <author>Vaidyanathan Nagarajan</author>
	      <year>2003</year>
	      <price>49.99</price>
	    </book>
	    <book category="web">
	      <title lang="en">Learning XML</title>
	      <author>Erik T. Ray</author>
	      <year>2003</year>
	      <price>39.95</price>
	    </book>
	  </bookstore>
	  <bookstore name="Mir">
	    <book category="classic">
	      <title lang="ru">Война и Мир</title>
	      <author>Лев Толстой</author>
	      <price>19.99</price>
	      <year>1869</year>
	    </book>
	    <book category="classic">
	      <title lang="ru">Тихй дон</title>
	      <author>Михаил Шолохов</author>
	      <price>19.95</price>
	      <year>1940</year>
	    </book>
	  </bookstore>
	  <!-- the descriptions of the bookstores -->
	  <bookstore xmlns="http://pum.donetsk.com/bookstore">
	    <name>Springler</name>
	    <address lang="en">
	      <country>US</country>
	      <location state="WG" city="Silver Spring" street="Wyane Ave, 12"/>
	    </address>
	  </bookstore>
	  <bookstore xmlns="http://pum.donetsk.com/bookstore">
	    <name>Mir</name>
	    <address lang="ru">
	      <country>Russian Federation</country>
	      <location state="Москва" city="Москва" street="Рижский 1-й пер., дом 2"/>
	    </address>
	  </bookstore>
	  <producer xmlns="http://pum.donetsk.com/bookstore">
	    <name>XYZ</name>
	  </producer>
	</document>
``

Result:
``
	  <bookstore xmlns="http://pum.donetsk.com/bookstore">
	    <name>Springler</name>
	    <address lang="en">
	      <country>US</country>
	      <location state="WG" city="Silver Spring" street="Wyane Ave, 12"/>
	    </address>
	  </bookstore><bookstore xmlns="http://pum.donetsk.com/bookstore">
	    <name>Mir</name>
	    <address lang="ru">
	      <country>Russian Federation</country>
	      <location state="Москва" city="Москва" street="Рижский 1-й пер., дом 2"/>
	    </address>
	  </bookstore>
``

# XPath example 3A

Task : same as in example 3

Document:

``
	<?xml version="1.0" encoding="UTF-8"?>
	<document xmlns:bsref="http://pum.donetsk.com/bookstore">
	  <bookstore name="Springler">
	    <book category="cooking">
	      <title lang="en">Everyday Italian</title>
	      <author>Giada De Laurentiis</author>
	      <year>2005</year>
	      <price>30.00</price>
	    </book>
	    <book category="children">
	      <title lang="en">Harry Potter</title>
	      <author>J K. Rowling</author>
	      <year>2005</year>
	      <price>29.99</price>
	    </book>
	    <book category="web">
	      <title lang="en">XQuery Kick Start</title>
	      <author>James McGovern</author>
	      <author>Per Bothner</author>
	      <author>Kurt Cagle</author>
	      <author>James Linn</author>
	      <author>Vaidyanathan Nagarajan</author>
	      <year>2003</year>
	      <price>49.99</price>
	    </book>
	    <book category="web">
	      <title lang="en">Learning XML</title>
	      <author>Erik T. Ray</author>
	      <year>2003</year>
	      <price>39.95</price>
	    </book>
	  </bookstore>
	  <bookstore name="Mir">
	    <book category="classic">
	      <title lang="ru">Война и Мир</title>
	      <author>Лев Толстой</author>
	      <price>19.99</price>
	      <year>1869</year>
	    </book>
	    <book category="classic">
	      <title lang="ru">Тихй дон</title>
	      <author>Михаил Шолохов</author>
	      <price>19.95</price>
	      <year>1940</year>
	    </book>
	  </bookstore>
	  <!-- the descriptions of the bookstores -->
	  <bsref:bookstore>
	    <bsref:name>Springler</bsref:name>
	    <bsref:address lang="en">
	      <bsref:country>US</bsref:country>
	      <bsref:location state="WG" city="Silver Spring" street="Wyane Ave, 12"/>
	    </bsref:address>
	  </bsref:bookstore>
	  <bsref:bookstore xmlns="http://pum.donetsk.com/bookstore">
	    <bsref:name>Mir</bsref:name>
	    <bsref:address lang="ru">
	      <bsref:country>Russian Federation</bsref:country>
	      <bsref:location state="Москва" city="Москва" street="Рижский 1-й пер., дом 2"/>
	    </bsref:address>
	  </bsref:bookstore>
	  <bsref:producer>
	    <bsref:name>XYZ</bsref:name>
	  </bsref:producer>
	</document>
``

XPath 1 : `/document/*[local-name()='bookstore' and namespace-uri()='http://pum.donetsk.com/bookstore']`

Result : same(!)

XPath 2 : `/document/bsref:bookstore`

Result : mostly the same

