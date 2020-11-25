# GWWikiDailyReminder
CLI app that parses Guild Wars Wiki and sends a daily e-mail reminder with a daily Zaishen quest list and Weekly active bonuses. 

To achieve proper functionality of the program, the program requires the user to input outgoing e-mail (SMTP) server parameters and the recepient address in the provided smtpParametersTemplate.json. Having filled out the said file, when calling the app, the path to the filled json file needs to be provided as a command line argument.
