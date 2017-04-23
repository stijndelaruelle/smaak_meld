<?php 
    require("mysql_info.php");
    require("utility.php");

    //Strings must be escaped to prevent SQL injection attack. 
    $name = mysql_real_escape_string($_GET['n']);
    $password = $_GET['p'];
    $hash = $_GET['h'];

    $error = "";
    if(CheckHash($name, $password, $hash, $error))
    {
        //Check if this username already exists
        $query = "SELECT * FROM Flavour_Users WHERE name = '$name'";
        $result = mysql_query($query) or die("Query failed: " . mysql_error());
     
        $num_results = mysql_num_rows($result);  
     
        if ($num_results == 0)
        {
            //Hash the password
            $passwordHash = password_hash($password, PASSWORD_BCRYPT); //MD password gets encrypyted once again
            $timestamp = date("Y-m-d H:i:s");

            //Add user to the database
            mysql_query("INSERT INTO Flavour_Users (id, name, password, licence_id, last_login_timestamp, register_timestamp)
                         VALUES('', '$name', '$passwordHash', '1', '', '$timestamp')") or die($error .= "E_Query failed: " . mysql_error() . "\n"); //1 for licence is TEMP
        }
        else
        {
            $error = "E_Username already taken.\n";
        }
    }

    echo $error;
?>