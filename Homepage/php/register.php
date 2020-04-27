<?php 
/**
 * #############################################################################
 * #                                                                           #
 * # copyright (c) 2019 marcos software, all rights reserved                   #
 * #                                                                           #
 * # this file may not be redistributed in whole or significant part           #
 * # and is subject to the marcos software license.                            #
 * #                                                                           #
 * # @author: marcos software, <info@marcos-software.de>                       #
 * # @copyright: 2019, marcos-software.de, http://www.marcos-software.de       #
 * #                                                                           #
 * #############################################################################
 */ 
 
    ini_set('session.cookie_lifetime', 60 * 60 * 24 * 7);  // 7 day cookie lifetime
 
	session_start();

	require_once('db.php');
	$pdo       = getPDO(1);

	$showFormular = true;
	 
	if(isset($_GET['register'])) {
				
		$error    		   = false;
		$email    		   = $_POST['email'];
		$passwort 		   = $_POST['passwort'];
		$passwort2 		   = $_POST['passwort2'];
		$recaptchaResponse = $_POST['g-recaptcha-response'];

		if(is_null($recaptchaResponse) || !isset($recaptchaResponse) || trim($recaptchaResponse) === '') {
			
			$errorMessage = 'ReCaptcha was not valid.';
			$error        = true;
		}	
	
		require_once('recaptcha.php');	
	
		if(!$error && !checkReCaptcha($recaptchaResponse)) {
			
			$errorMessage = 'ReCaptcha was not valid.';
			$error        = true;
		}
		
		if(!$error && !filter_var($email, FILTER_VALIDATE_EMAIL)) {
			
			$errorMessage = 'E-Mail is not valid.';
			$error        = true;
		} 
		
		if(!$error && strlen($passwort) == 0) {
			
			$errorMessage = 'Choose a password.';
			$error        = true;
		}
		
		if(!$error && $passwort != $passwort2) {
			
			$errorMessage = 'The passwords are not identical.';
			$error        = true;
		}
		
		if(!$error) { 
		
			$statement = $pdo->prepare("SELECT * FROM vUsers WHERE cMail = :email");
			$result    = $statement->execute(array('email' => $email));
			$user      = $statement->fetch();
			
			if($user !== false) {
				
				$errorMessage = 'E-Mail is already in use.';
				$error        = true;
			}    
		}
		
		if(!$error) {    
		
			$passwort_hash = password_hash($passwort, PASSWORD_DEFAULT);
			
			$stmt = $pdo->prepare("CALL spCreateUser(?, ?)");
			
			$stmt->bindParam(1, $email, PDO::PARAM_STR, 50);
			$stmt->bindParam(2, $passwort_hash, PDO::PARAM_STR, 255);
			
			$result = $stmt->execute();

			if($result) {  
			
				$successMessage = '<strong>User created</strong><br/>Please contact <a href="mailto:marc@marcos-software.de">marc@marcos-software.de</a> for activation.';
				$showFormular   = false;
			} else {
				
				$errorMessage = '<strong>Error on adding user</strong><br />Please contact <a href="mailto:marc@marcos-software.de">marc@marcos-software.de</a>.';
			}
		} 
	}
?>

<html lang="en"> 

<?php

	readfile("../html/head.html");
	echo "<body style='padding-top: 40px; padding-bottom: 40px; background-color: #eee;'>";

	if(isset($successMessage)) {
		
		echo '<div class="alert alert-success">';
		echo "$successMessage";
		echo '</div>';
	}
	
	if(isset($errorMessage)) {
		
		echo '<div class="alert alert-danger">';
		echo "$errorMessage";
		echo '</div>';
	}

	if($showFormular) {
		
		echo '<form class="form-signin" action="?register=1&'.$_SERVER['QUERY_STRING'].'" method="post">';	
		readfile("../html/register.html");
		echo "<br/><br/>";
		readfile("../html/footer.html");
		echo '</form>';
	}
?>		
	</body>
</html>