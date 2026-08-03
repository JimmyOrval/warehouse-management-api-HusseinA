ex08 - security review of the upload-invoice snippet

There is no authentication on the endpoint

Having file.Filename alone without any checks is a big security issue, where hackers
can have a filename that can keep sensitive files vulnerable.
Also there's no file type check, so anyone can post a .exe and break things.

There is no file size limit.

It should throw exceptions when the input is not as intended

Files save directly to wwwroot, which doesn't need auth to access, so sensitive
data is at risk, like invoices

FileMode.Create directly creates the file without checking if the name already exists,
so overwrites can accidentally happen

The response contains the fullPath, which exposes folder structure

There is no logging at all