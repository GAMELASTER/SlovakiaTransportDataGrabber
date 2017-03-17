# SlovakiaTransportDataGrabber
It's parser of data from unnamed Slovakian Cities Transport website.
By using this parser you might be violating unnamed site ***Terms of Services***
# Some notices
- This code has been writed two years ago, where I has refactoring a three years old code, so it's garbage code
- Probably it's not working today, I didn't tested it
# Usage
1. Create DATA directory along the executable file
2. Create XML named by city shortcut, for sample `BB.xml` , and put there a code with routes. Example:
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<region shortcut="BB" name="Banská Bystrica">
	<route number="1" type="electricbus"/>
	<route number="2" type="electricbus"/>
	<route number="3" type="electricbus"/>
	<route number="4" type="electricbus"/>
	<route number="5" type="electricbus"/>
	<route number="6" type="electricbus"/>
	<route number="7" type="electricbus"/>
	<route number="8" type="electricbus"/>
	<route number="20" type="normalmhdbus"/>
	<route number="21" type="normalmhdbus"/>
	<route number="22" type="normalmhdbus"/>
	<route number="23" type="normalmhdbus"/>
	<route number="24" type="normalmhdbus"/>
	<route number="25" type="normalmhdbus"/>
	<route number="26" type="normalmhdbus"/>
	<route number="27" type="normalmhdbus"/>
	<route number="28" type="normalmhdbus"/>
	<route number="29" type="normalmhdbus"/>
	<route number="32" type="normalmhdbus"/>
	<route number="33" type="normalmhdbus"/>
	<route number="34" type="normalmhdbus"/>
	<route number="35" type="normalmhdbus"/>
	<route number="36" type="normalmhdbus"/>
	<route number="41" type="normalmhdbus"/>
	<route number="42" type="normalmhdbus"/>
	<route number="43" type="normalmhdbus"/>
	<route number="80" type="normalmhdbus"/>
	<route number="90" type="normalmhdbus"/>
	<route number="97" type="normalmhdbus"/>
	<route number="100" type="normalmhdbus"/>
	<route number="601451" type="regionmhdbus"/>
	<route number="601454" type="regionmhdbus"/>
	<route number="601455" type="regionmhdbus"/>
	<route number="601456" type="regionmhdbus"/>
	<route number="601457" type="regionmhdbus"/>
	<route number="601458" type="regionmhdbus"/>
	<route number="601460" type="regionmhdbus"/>
	<route number="601462" type="regionmhdbus"/>
	<route number="601463" type="regionmhdbus"/>
	<route number="601464" type="regionmhdbus"/>
	<route number="601465" type="regionmhdbus"/>
	<route number="601466" type="regionmhdbus"/>
	<route number="601467" type="regionmhdbus"/>
	<route number="601468" type="regionmhdbus"/>
	<route number="601469" type="regionmhdbus"/>
	<route number="601470" type="regionmhdbus"/>
</region>
```
3. Follow the program steps
4. Watch & Enjoy
