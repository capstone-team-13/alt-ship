param (
    [string]$author,
    [string]$since,
    [string]$until
)

$adds = 0
$dels = 0
$commitCount = 0

$commitHashes = git log --author="$author" --since="$since" --until="$until" --pretty="%H"
foreach ($hash in $commitHashes) {
    $commitMessage = git log -1 --format=%B -n 1 $hash
    if ($commitMessage -notlike "*Install*" -and $commitMessage -notlike "*Merge*") {
        $commitCount++
        $stats = git show --numstat $hash | Select-String "^\d+\s+\d+\s+"
        foreach ($line in $stats) {
            $fields = $line -split '\s+'
            if ($fields.Length -eq 3) {
                $adds += [int]$fields[0]
                $dels += [int]$fields[1]
            }
        }
    }
}
Write-Host "$([char]27)[96mCommits: $commitCount$([char]27)[0m $([char]27)[92m+Additions: $adds$([char]27)[0m $([char]27)[91m-Deletions: $dels$([char]27)[0m"
