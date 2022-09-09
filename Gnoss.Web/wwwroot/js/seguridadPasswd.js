// Password strength meter v1.0
// Matthew R. Miller - 2007
// www.codeandcoffee.com
// Based off of code from  http://www.intelligent-web.co.uk

// Settings
// -- Toggle to true or false, if you want to change what is checked in the password
var bCheckNumbers = true;
var bCheckUpperCase = true;
var bCheckLowerCase = true;
var bCheckPunctuation = true;
var nPasswordLifetime = 365;

// Check password
//function checkPassword(strPassword)
function checkPassword(p)
{
var intScore = 0;

			// PASSWORD LENGTH
			intScore += p.length;
			
			if(p.length > 0 && p.length <= 4) {                    // length 4 or less
				intScore += p.length;
			}
			else if (p.length >= 5 && p.length <= 7) {	// length between 5 and 7
				intScore += 6;
			}
			else if (p.length >= 8 && p.length <= 15) {	// length between 8 and 15
				intScore += 12;
				//alert(intScore);
			}
			else if (p.length >= 16) {               // length 16 or more
				intScore += 18;
				//alert(intScore);
			}
			
			// LETTERS (Not exactly implemented as dictacted above because of my limited understanding of Regex)
			if (p.match(/[a-z]/)) {              // [verified] at least one lower case letter
				intScore += 1;
			}
			if (p.match(/[A-Z]/)) {              // [verified] at least one upper case letter
				intScore += 5;
			}
			// NUMBERS
			if (p.match(/\d/)) {             	// [verified] at least one number
				intScore += 5;
			}
			if (p.match(/.*\d.*\d.*\d/)) {            // [verified] at least three numbers
				intScore += 5;
			}
			
			// SPECIAL CHAR
			if (p.match(/[!,@,#,$,%,^,&,*,?,_,~]/)) {           // [verified] at least one special character
				intScore += 5;
			}
			// [verified] at least two special characters
			if (p.match(/.*[!,@,#,$,%,^,&,*,?,_,~].*[!,@,#,$,%,^,&,*,?,_,~]/)) {
				intScore += 5;
			}
			
			// COMBOS
			if (p.match(/(?=.*[a-z])(?=.*[A-Z])/)) {        // [verified] both upper and lower case
				intScore += 2;
			}
			if (p.match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])/)) { // [verified] both letters and numbers
				intScore += 2;
			}
	 		// [verified] letters, numbers, and special characters
			if (p.match(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!,@,#,$,%,^,&,*,?,_,~])/)) {
				intScore += 2;
			}

			return intScore;
//	// Reset combination count
//	nCombinations = 0;
//	
//	// Check numbers
//	if (bCheckNumbers)
//	{
//		strCheck = "0123456789";
//		if (doesContain(strPassword, strCheck) > 0) 
//		{ 
//        		nCombinations += strCheck.length; 
//    		}
//	}
//	
//	// Check upper case
//	if (bCheckUpperCase)
//	{
//		strCheck = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
//		if (doesContain(strPassword, strCheck) > 0) 
//		{ 
//        		nCombinations += strCheck.length; 
//    		}
//	}
//	
//	// Check lower case
//	if (bCheckLowerCase)
//	{
//		strCheck = "abcdefghijklmnopqrstuvwxyz";
//		if (doesContain(strPassword, strCheck) > 0) 
//		{ 
//        		nCombinations += strCheck.length; 
//    		}
//	}
//	
//	// Check punctuation
//	if (bCheckPunctuation)
//	{
//		strCheck = ";:-_=+\|//?^&!.@$£#*()%~<>{}[]";
//		if (doesContain(strPassword, strCheck) > 0) 
//		{ 
//        		nCombinations += strCheck.length; 
//    		}
//	}
//	
//	// Calculate
//	// -- 500 tries per second => minutes 
//    	var nDays = ((Math.pow(nCombinations, strPassword.length) / 500) / 2) / 86400;
// 
//	// Number of days out of password lifetime setting
//	var nPerc = nDays / nPasswordLifetime;
//	
//	return nPerc;
}
 
// Runs password through check and then updates GUI 
function runPassword(strPassword, strFieldID) 
{
	// Check password
	nPerc = checkPassword(strPassword);
	
	 // Get controls
    	var ctlBar = document.getElementById("pwbar"); 
    	var ctlText = document.getElementById("pwtext");
    	if (!ctlBar || !ctlText)
    		return;
    	
    	var nRound = Math.round(nPerc * 2);

			if (nRound > 100) {
				nRound = 100;
			}
        ctlBar.style.width = nRound + "%";
			
			// Color and text
 	if (nRound > 75)
 	{
 		strText = form.muysegura;
 		strColor = "#3bce08";
 	}
 	else if (nRound > 50)
 	{
 		strText = form.segura;
 		strColor = "#c8c800";
	}
 	else if (nRound > 25)
 	{
 		strText = form.pocosegura;
 		strColor = "orange";
 	}
 	else
 	{
 	    strText = form.insegura;
 		strColor = "red";
 	}
	ctlBar.style.backgroundColor = strColor;
	ctlText.innerHTML = "<span style='color: " + strColor + ";'>" + strText + "</span>";
    	
    	
//    	// Set new width
//    	var nRound = Math.round(nPerc * 100);
//	if (nRound < (strPassword.length * 5)) 
//	{ 
//		nRound += strPassword.length * 5; 
//	}
//	if (nRound > 100)
//		nRound = 100;
//    	ctlBar.style.width = nRound + "%";
// 
// 	// Color and text
// 	if (nRound > 95)
// 	{
// 		strText = "Muy segura";
// 		strColor = "#3bce08";
// 	}
// 	else if (nRound > 75)
// 	{
// 		strText = "Segura";
// 		strColor = "orange";
//	}
// 	else if (nRound > 50)
// 	{
// 		strText = "Poco segura";
// 		strColor = "#ffd801";
// 	}
// 	else
// 	{
// 	    strText = "Insegura";
// 		strColor = "red";
// 	}
//	ctlBar.style.backgroundColor = strColor;
//	ctlText.innerHTML = "<span style='color: " + strColor + ";'>" + strText + "</span>";
}
 
// Checks a string for a list of characters
function doesContain(strPassword, strCheck)
 {
    	nCount = 0; 
 
	for (i = 0; i < strPassword.length; i++) 
	{
		if (strCheck.indexOf(strPassword.charAt(i)) > -1) 
		{ 
	        	nCount++; 
		} 
	} 
 
	return nCount; 
} 
 

