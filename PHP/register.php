<?php 
    require("mysql_info.php");

    //Strings must be escaped to prevent SQL injection attack. 
    $name = mysql_real_escape_string($_GET['n']);
    $password = $_GET['p'];
    $hash = $_GET['h'];

    $secretKey = "afzlearvtoyuurimpeqlsddkfagmhejrk";
    $real_hash = md5($name . $secretKey . $password);

    $error = "";
    if($hash == $real_hash)
    {
        //Check if this username already exists
        $query = "SELECT * FROM Flavour_Meld WHERE name = '$name'";
        $result = mysql_query($query) or die("Query failed: " . mysql_error());
     
        $num_results = mysql_num_rows($result);  
     
        if ($num_results == 0)
        {
            //Hash the password
            $passwordHash = password_hash($password, PASSWORD_BCRYPT); //MD password gets encrypyted once again
            $timestamp = date("Y-m-d H:i:s");

            mysql_query("INSERT INTO Flavour_Meld (id, name, password, last_login_timestamp, register_timestamp)
                         VALUES('', '$name', '$passwordHash', '', '$timestamp')") or die("Query failed: " . mysql_error());
        }
        else
        {
            $error = "Username already taken.";
        }
    }
    else
    {
        $error = "Hashes do not match. What are you doing, trying to hack this system?";
    }

    echo $error;
?>