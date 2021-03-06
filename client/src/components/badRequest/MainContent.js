import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';

const styles = () => ({
  allStyle: {
    margin: '0',
    padding: '0'
  },
  bodyStyle: {
    background: '#22223b',
    height: '93.5vh',
    borderRadius: '40'
  },
  whistle: {
    width: '20%',
    fill: '#4a4e69',
    margin: '100px 40%',
    textAlign: 'left',
    transform: 'rotate(0)',
    transformOrigin: '80% 30%',
    animation: 'wiggle .2s infinite'
  },
  h1Style: {
    marginTop: '-100px',
    marginBottom: '20px',
    color: '#4a4e69',
    textAlign: 'center',
    fontSize: '140px',
    fontWeight: '800',
    fontFamily: 'Open Sans',
  },
  h2Style: {
    color: '#4a4e69',
    textAlign: 'center',
    fontSize: '50px',
    fontFamily: 'Open Sans',
  },
  pStyle: {
    color: '#4a4e69',
    textAlign: 'center',
    fontSize: '32px',
    fontFamily: 'Open Sans',
  },
  noteStyle: {
    color: '#4a4e69',
    textAlign: 'center',
    fontSize: '16px',
    marginTop: '10px',
    fontFamily: 'Open Sans',
  }
});

function BadRequestContent({ classes }) {
  return (
    <div className={`${classes.bodyStyle} ${classes.allStyle}`}>
      <img src="/static/icons/dizzy.svg" className={`${classes.whistle}`} alt="dizzy" />
      <h1 className={classes.h1Style}>400</h1>
      <h2 className={classes.h2Style}>The request sent was not valid</h2>
      <p className={classes.pStyle}>Please check the URL once again for correctness</p>
      <p className={classes.noteStyle}>
        Pssstt, did you forget to give the three arguments?
        <strong> Domain</strong>
        ,
        <strong>ClientID</strong>
        ,
        <strong>RequestID?</strong>
      </p>
    </div>
  );
}

BadRequestContent.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(BadRequestContent);
