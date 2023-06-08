# Browser Driver Example

An example of the C# code you can use to run and control a Chromium-based web browser using its DevTools APIs. Basically a very simple re-imagining of test automation tools like Playwright or Selenium.

This is the source go with a [blog post series](???) I wrote, about how this process can work. 

Note that the first time you run this code you may find that it does not successfully automate the browser. Particularly with Edge, the "startup" behaviour for a new profile (or if there's been a big security patch) seems to prevent the DevTools API working until after you've answered all the "can we enable this feature?" questions that get asked. In that scenario you can supply the relevant answers to the browser, close that and the console app, and then run the console app again. It should work fine once it doesn't need to ask you questions...