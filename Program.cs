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

}

