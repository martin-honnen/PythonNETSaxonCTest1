using Python.Runtime;

PythonEngine.Initialize();

using var _ = Py.GIL();

dynamic saxonche = Py.Import("saxonche");

using (dynamic saxonproc = saxonche.PySaxonProcessor())
{
    Console.WriteLine(saxonproc.version);

    Console.WriteLine(saxonproc.new_xpath_processor().evaluate_single("current-dateTime()"));

    dynamic xslt30Processor = saxonproc.new_xslt30_processor();

    dynamic xslt30Transformer = xslt30Processor.compile_stylesheet(stylesheet_text: @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='3.0' expand-text='yes'>
<xsl:output indent='yes'/>
<xsl:template name='xsl:initial-template'>
  <root>This is a test with {system-property('xsl:product-name')} {system-property('xsl:product-version')} at {current-dateTime()}</root>
</xsl:template>
</xsl:stylesheet>");

    var result = xslt30Transformer.call_template_returning_string(template_name: null);

    Console.WriteLine(result);

    xslt30Transformer = xslt30Processor.compile_stylesheet(stylesheet_text: @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='3.0' expand-text='yes'>
<xsl:output indent='yes' build-tree='no'/>
<xsl:template name='xsl:initial-template'>
  <xsl:for-each select='1 to 5'>
    <xsl:document>
      <root>This is test {.} with {system-property('xsl:product-name')} {system-property('xsl:product-version')} at {current-dateTime()}</root>
    </xsl:document>
  </xsl:for-each>
</xsl:template>
</xsl:stylesheet>");

    xslt30Transformer.set_result_as_raw_value(true);

    dynamic xdmValue = xslt30Transformer.call_template_returning_value(template_name: null);

    foreach (dynamic xdmItem in xdmValue)
    {
        Console.WriteLine(xdmItem);
    }

}

