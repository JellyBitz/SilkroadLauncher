# Language Support Guide

Google sheet with all languages organized and translated right to be used with the client/launcher.
Link: https://docs.google.com/spreadsheets/d/1qHcztTx9MsBwl3fyKYWWhHFg92anqWkbs-mGbxQGSQg


## How to use it?

1. Delete the data from columns which languages you're not going to use
2. Select each sheet and download them with .tsv format
3. Rename the files from `Silkroad Text - FILENAME.tsv` to `FILENAME.txt`
4. Convert all texts downloaded from `UTF-8` to `UTF-16 LE with BOM` handled by client (You could use Sublime Text)
5. Import them into client media.pk2 at `server_dep/silkroad/textdata/` path using any PK2 tool