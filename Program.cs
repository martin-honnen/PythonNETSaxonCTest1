using Python.Runtime;

Console.WriteLine($".NET {Environment.Version} on {Environment.OSVersion}");

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


    xslt30Transformer = xslt30Processor.compile_stylesheet(stylesheet_text: @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='3.0' expand-text='yes'>
<xsl:output indent='yes' build-tree='no'/>
<xsl:template name='xsl:initial-template'>
  <principal-result>
    <root>This is a test with {system-property('xsl:product-name')} {system-property('xsl:product-version')} at {current-dateTime()}</root>
  </principal-result>
  <xsl:for-each select='1 to 5'>
    <xsl:result-document href='result-{.}.xml'>
      <root>This is test {.} with {system-property('xsl:product-name')} {system-property('xsl:product-version')} at {current-dateTime()}</root>
    </xsl:result-document>
  </xsl:for-each>
</xsl:template>
</xsl:stylesheet>");

    xslt30Transformer.set_base_output_uri("urn:to-string");
    xslt30Transformer.set_result_as_raw_value(false);
    xslt30Transformer.set_capture_result_documents(true, false);

    xdmValue = xslt30Transformer.call_template_returning_value(template_name: null);

    Console.WriteLine(xdmValue);

    dynamic resultDocs = xslt30Transformer.get_result_documents();

    foreach (dynamic uri in resultDocs.keys())
    {
        Console.WriteLine("{0}:\n{1}", uri, resultDocs[uri]);
    }

    //dynamic parseJsonFn = saxonche.PyXdmFunctionItem().get_system_function(saxonproc, "{http://www.w3.org/2005/xpath-functions}parse-json", 1, "utf8");

    var exampleJSON = @"{ ""name"" : ""foo"", ""data"" : [1, 2, 3, 4, 5] }";

    //dynamic argumentList = new PyList();
    //argumentList.append(saxonproc.make_string_value(exampleJSON));

    //dynamic functionCallResult = parseJsonFn.call(saxonproc, argumentList);

    //Console.WriteLine(functionCallResult);

    dynamic functionCallResult = saxonproc.parse_json(json_text: exampleJSON);

    dynamic xpathProcessor = saxonproc.new_xpath_processor();

    xpathProcessor.set_context(xdm_item: functionCallResult.head);

    dynamic xdmNode = xpathProcessor.evaluate_single(". => serialize(map { 'method' : 'json' }) => json-to-xml()");

    Console.WriteLine(xdmNode);

    dynamic xqueryProcessor = saxonproc.new_xquery_processor();

    xqueryProcessor.set_query_content(@"declare namespace output = 'http://www.w3.org/2010/xslt-xquery-serialization';
      declare option output:method 'text';
      xml-to-json(., map { 'indent' : true() })");

    var xqueryResult = xqueryProcessor.run_query_to_value(input_xdm_item: xdmNode);

    Console.WriteLine(xqueryResult);


    // exception handling

    xslt30Processor = saxonproc.new_xslt30_processor();

    try
    {
        xslt30Transformer = xslt30Processor.compile_stylesheet(stylesheet_text: @"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='3.0' expand-text='yes'>
<xsl:output indent='yes'/>
<xsl:mode on-no-match='shallow-cpy'/>
<xsl:template name='xsl:initia-template'>
  <root>This is a test with {system-property('xsl:product-name')} {system-property('xsl:product-version')} at {current-dateTime()}</root>
</xsl:template>
</xsl:stylesheet>");

        result = xslt30Transformer.call_template_returning_string(template_name: null);

        Console.WriteLine(result);
    } 
    catch (Exception e)
    {
        Console.WriteLine($"Error compiling stylesheet: {e.Message}");
    }

    try
    {
        var xpathResult = xpathProcessor.evaluate_single("1 div 0");
        Console.Write(xpathResult);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error during XPath evaluation: {e.Message}");

    }

}

