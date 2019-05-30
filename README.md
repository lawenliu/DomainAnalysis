# MaRK

Table of Contents
-----------------
1. [Requirements](#requirements)
1. [Usage](#usage)
1. [Structure](#structure)
1. [Troubleshooting](#troubleshooting)

Requirements
------------
Requirement Knowledge Mining tool (MaRK) is developed based on .Net Framework 4.5. You are required to install .Net Framework 4.5 firstly before installing our tool.
MaRK also needs Office, Adobe Reader, Java, 7-Zip and R for implementing the related functions. We add some reference downloading URLs here:
* Java: http://www.oracle.com/technetwork/java/javase/downloads/jdk7-downloads-1880260.html (MaRK is using Stanford TMT for the topic modeling, which requires JDK 1.7). Tips:  If you fail at Running TMT step after you have setup JDK and added path to Environment Variable, you may need restart your computer and make sure your JDK can be accessed in normal command window instead of administrator command window.
*	R: https://cran.r-project.org/bin/windows/base/
*	7Zip: http://www.7-zip.org/ 
*	Adobe Reader: https://get.adobe.com/reader/ 


Usage
------------------
## Manual Model Based
1. Open MaRK

1. Click Operations menu and select “Manual Model-based” item to start to show the existing manual model. Then “Select Source File” dialog is opened.


1. In Select Source File dialog, select the directory containing the domain documents (such as file with extension: pdf, doc or docx) and the domain model which is created with tool FreeMind and stored as .xml. If you have pre-defined search terms, you can also import it.

1. You can click button Start New Computing to run the domain model creation process, it will delete the files on your last result if you have. Or you click the button Open Last Result to review your last result. The following is about the new computing to find the component relevant documents. 

1. The running time depends on the file count under domain model folder you selected.
1. Once all computing finished, the domain will be shown in tree (in the left screen). To show the component-related documents and their ranking, you need click one component in the model. 

1. Double click the component, MaRK will generate the summary from all of the related components. 
1. Double click one related document, MaRK will give the page index of all relevant part in the bottom. If you click one page, you can look at the specific page. Or you can also click I feel lucky, MaRK will identify the most relevant page and open that page for you.

## Automated Model-based

1. Startup MaRK.
1. Click Operations menu and select “Automated Model-based” item to start the requirements knowledge mining based on the automated model creation. Due to the complexity of this process, we create one wizard for this process.

1. Firstly, you need to select the directory of your domain documents. And then MaRK will copy the documents to the temporary folder of this application.

1. Then MaRK will extract PDF file to text.

1. The running time depends on the file count under domain model folder you selected.
1. Once the model creation and document ranking finished, the domain will be shown in hierarchy.

1.	Once the model has been created, the process for generating the highlighting and summary is same to that with manual model.

Structure
---------

Troubleshooting
---------------
Developers: Xiaoli Lian, Li Zhang, Wenchuang Liu
If you have any problem in setup or using, please create issues to us.

