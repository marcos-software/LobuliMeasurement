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

	if(isset($_GET['login'])) {

		$email             = $_POST['email'];
		$passwort          = $_POST['passwort'];
		$recaptchaResponse = $_POST['g-recaptcha-response'];
		
		$error = false;
		
		if(is_null($recaptchaResponse) || !isset($recaptchaResponse) || trim($recaptchaResponse) === '') {
			
			$errorMessage = 'ReCaptcha was not valid.';
			$error        = true;
			
		} else {	
	
			require_once('recaptcha.php');	
		
			if(!checkReCaptcha($recaptchaResponse)) {
				
				$errorMessage = 'ReCaptcha was not valid.';
				$error        = true;
			}
		}
		
		if(!$error) {

			$statement         = $pdo->prepare("SELECT * FROM vUsers WHERE cMail = :email");
			$result            = $statement->execute(array('email' => $email));
			$user              = $statement->fetch();

			if ($user !== false && password_verify($passwort, $user['cPasswordHash'])) {
				
				$_SESSION['userid'] = $user['cUserId'];
				
				if(isset($_POST['angemeldet_bleiben'])) {
				 
					require_once('utils.php');
					 
					$identifier    = random_string();
					$securitytoken = random_string();

					$stmt          = $pdo->prepare("CALL spInsertToken(?, ?, ?)");

					$stmt->bindParam(1, $user['cUserId'], PDO::PARAM_INT, 11);
					$stmt->bindParam(2, $identifier, PDO::PARAM_STR, 255);
					$stmt->bindParam(3, sha1($securitytoken), PDO::PARAM_STR, 255);

					$result = $stmt->execute();

					setcookie("identifier",$identifier,time()+(60*60*24*7), "", "", true, true);
					setcookie("securitytoken",$securitytoken,time()+(60*60*24*7), "", "", true, true);

				}
				
				header("Location: https://YOUR_BASE_URL_HERE.com/php/index.php?".$_SERVER['QUERY_STRING']); 
				header("Connection: close"); 
				
			} else {
				
				$errorMessage = '<strong>Ah Ah Ah</strong><br />Du hast das Zauberwort nicht gesagt!<br>';
			}  
		}		
	}
?>

<html lang="en"> 

<?php 

	readfile("../html/head.html");
	echo "<body style='padding-top: 40px; padding-bottom: 40px; background-color: #eee;'>";
	
	if(isset($errorMessage)) {
		
		echo '<div class="alert alert-danger">';
		echo "$errorMessage";
		echo '</div>';
	}
	
	echo '<form class="form-signin" action="?login=1&'.$_SERVER['QUERY_STRING'].'" method="post">';	
	readfile("../html/login.html");
	echo "<br/><br/>";
	readfile("../html/footer.html");
?>		
	</body>
</html>

