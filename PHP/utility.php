<?php

    require("mysql_info.php");

    function CheckHash($name, $password, $hash, &$error)
    { 
        $secretKey = "afzlearvtoyuurimpeqlsddkfagmhejrk"; //Hardcoded
        $real_hash = md5($name . $secretKey . $password);

        if ($hash == $real_hash)
        {
            return true;
        }
        else
        {
            $error .= "E_Hashes do not match. What are you doing, trying to hack this system?\n";
        }

        return false;
    }

    function CheckLogin($name, $password, &$error)
    {
        //Check if we can login
        $query = "SELECT * FROM Flavour_Users WHERE name='$name'";
        $result = mysql_query($query) or die($error .= "E_Query failed: " . mysql_error() . "\n");
     
        $num_results = mysql_num_rows($result);  
     
        if ($num_results == 0)
        {
            $error .= "E_Invalid username.\n";
        }

        for($i = 0; $i < $num_results; $i++)
        {
            $row = mysql_fetch_array($result);
            $passwordHash = $row['password'];

            if (password_verify($password, $passwordHash))
            {
                return $row;
            }
            else
            {
                $error .= "E_Invalid password.\n";
            }
        }

        return null;
    }

?>